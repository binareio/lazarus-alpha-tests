#r "Newtonsoft.Json"
#load "sql.csx"
#load "../Get-Seguimiento-Observacion/sql.csx"

using System;
using System.Globalization;
using System.Net;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

//using System;
//using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;

static readonly HttpClient httpclient = new HttpClient();

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.222");

    var requestHeader = req.Headers;
    var vvapikeysecure = Environment.GetEnvironmentVariable("apikey", EnvironmentVariableTarget.Process);
    //string vvapikeysecure = requestHeader["apiKey"];
    string vvapiKeyparameter = requestHeader["apiKey"];

    string vvhttpmethod = req.Query["httpmethod"];

    string jsonrspt = "";

    if (vvapikeysecure != vvapiKeyparameter)
    {
        vvhttpmethod = "";
        jsonrspt = "{}";
    }

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic body = JsonConvert.DeserializeObject(requestBody);
    log.LogInformation("body -> ");

    SendMailDA sendMailDA = new SendMailDA();
    SeguimientoObs seguimientoObs = new SeguimientoObs();

    if (vvhttpmethod == "post")
    {
        seguimientoObs = fnSetObjSeguimiento(log, body);

        long idSE = 0;
        string tipo = "";
        if (seguimientoObs.Sede == null || seguimientoObs.Sede == 0)
        {
            idSE = seguimientoObs.Embarcacion;
            tipo = "E";
        }
        else
        {
            idSE = seguimientoObs.Sede;
            tipo = "S";
        }

        log.LogInformation("TIPO -> "+tipo);
        log.LogInformation("idSE -> "+idSE);
        log.LogInformation("seguimientoObs.Updated_By -> "+seguimientoObs.Updated_By);

        string emails = await sendMailDA.fnGetCorreos(idSE, tipo);

        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            try
            {
                int rows = await sendMailDA.fnSeguimientoUpdateInforme(seguimientoObs);
                if (rows > 0)
                {
                    SeguimientoDA parametricaDA = new SeguimientoDA();
                    Result objs = await parametricaDA.fnGetSeguimientoObject(seguimientoObs.Id);

                    enviarCorreo(log, seguimientoObs, objs, emails);
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                log.LogInformation("catch::" + ex.Message);
                seguimientoObs.Id = -1;
            }

        Response response = new Response();
        response.status = true;
        response.message = "Se envi� el correo";

        jsonrspt = JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    if (vvhttpmethod == "prueba")
    {
        log.LogInformation("en vvhttpmethod == prueba INICIO");

        Account oAccount = new Account();

        DataEmailNew oDataEmail = new DataEmailNew();
        // destinatarios del email
        string correos = "orlyvila@visualsat.com,jleandrovc@gamil.com,jmendoza@visualsat.com,jmillan@visualsat.com";
        oDataEmail.sendto = "millanqjesus@gmail.com";
        oDataEmail.from = "sigtasa@tasa.com.pe";

        // ASUNTO DEL EMAIL
        oDataEmail.emailsubject = "BETA Informe de OP";

        // cuerpo del emal
        string bodyEmail = "<div>Hola que tal...</div>";
        oDataEmail.bodyhtml = bodyEmail;

        oAccount = sendEmail(log, oDataEmail);

        log.LogInformation("en vvhttpmethod == prueba FINAL");
        jsonrspt = JsonConvert.SerializeObject(oAccount, Formatting.Indented);
    }

    return jsonrspt != null
        ? (ActionResult)new OkObjectResult($"{jsonrspt}")
        : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
}



public static SeguimientoObs fnSetObjSeguimiento(ILogger log, dynamic body)
{
    SeguimientoObs obj = new SeguimientoObs();
    
    log.LogInformation("body -> ");

    obj.Id = body?.Id;
    log.LogInformation("body Id -> "+obj.Id);
    obj.Pdf = body?.Pdf;
    log.LogInformation("body Pdf -> ");
    obj.Codigo = body?.Codigo;    
    log.LogInformation("body Codigo -> ");
    obj.Tipo_Observacion = body?.Tipo_Observacion;
    log.LogInformation("body Tipo_Observacion -> ");
    obj.Sede = body?.Sede;
    log.LogInformation("body Sede -> ");
    obj.Embarcacion = body?.Embarcacion;
    log.LogInformation("body Embarcacion -> ");
    obj.Area = body?.Area;
    log.LogInformation("body Area -> ");
    obj.Zona = body?.Zona;
    log.LogInformation("body Zona -> ");
    obj.Codigo_Reportante = body?.Codigo_Reportante;
    obj.Nombres_Reportante = body?.Nombres_Reportante;
    obj.Codigo_Reportado = body?.Codigo_Reportado;
    obj.Nombres_Reportado = body?.Nombres_Reportado;
    obj.Updated_By = body?.Updated_By;
    obj.Estado = body?.Estado;
    obj.Correos = body?.Correos;
        
    log.LogInformation("obj.Id -> "+ obj.Id);
    log.LogInformation("obj.Correos -> "+ obj.Correos);
    log.LogInformation("obj.Embarcacion -> "+ obj.Embarcacion);

    //TimeZoneInfo PeruZona = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
    //DateTime horaPeru = TimeZoneInfo.ConvertTime(DateTime.Now, PeruZona);

    //obj.Fecha_Operacion = Convert.ToDateTime(horaPeru.ToShortTimeString());
    //obj.Hora_Operacion = horaPeru.ToShortTimeString();

    return obj;
}


public static async void enviarCorreo(ILogger log, SeguimientoObs obj, Result resul, string emails)
{
    Seguimiento ent = resul.DatosPrincipales;
    log.LogInformation("obj.Correos " + obj.Correos);
    log.LogInformation("emails  " + emails);
    //string correos = emails + obj.Correos + "orlyvila@visualsat.com,jvillarroel@visualsat.com,jmendoza@visualsat.com, pravichahua@visualsat.com, jamedi2011@gmail.com";
    string correos = "orlyvila@visualsat.com,jvillarroel@visualsat.com,jmendoza@visualsat.com,jmillan@visualsat.com";
    DataEmailNew oDataEmail = new DataEmailNew();
    oDataEmail.sendto = correos;
    oDataEmail.from = "sigtasa@tasa.com.pe";

    // ASUNTO DEL EMAIL
    oDataEmail.emailsubject = "Informe de OP";

    //CUERPO DEL CORREO
    //var bodyEmail = "<body style='color: #ccc'><table border='0' cellpadding='0' cellspacing='0' style='max-width:100%;width:60%;margin: 0 auto;' id='tablota' ><tbody>";
    //bodyEmail += "<tr>";
    //bodyEmail += "<td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' style='background-color:#fff;border-bottom:1px solid #ccc;font-family:Arial;font-weight:bold;line-height:100%;vertical-align:middle' width='100%'>";
    //bodyEmail += "<tbody><tr><td style='padding-bottom: 15px;'><a  href='https://sigtasa.tasa.com.pe/'  target='_blank' title='TasaSsoma'> <img id='logo' alt='visitasa' src='https://sigtasa.tasa.com.pe/images/logoSIGTASA.png' width ='200px' height ='90px'></a></td></tr></tbody></table>";
    //bodyEmail += "</td>";
    //bodyEmail += "</tr>";
    //bodyEmail += "<tr>";
    //bodyEmail += "<td align='center'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td style='background-color:#fff' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td><div style='color:#444 !important;font-family:Arial;font-size:14px;line-height:150%;text-align:left'>";
    //bodyEmail += "<p>Estimado, saludos cordiales:</p>";
    //bodyEmail += "<p>Adjunto el informe de Seguimiento de Observaci&oacute;n</p>";
    //bodyEmail += "<p>Reportante: <strong>" + obj.Nombres_Reportante + "</strong></p>";
    //bodyEmail += "<p><a href='https://www.visualsatpe.com/beta/tasassoma/view/download.html?Id=" + obj.Id + "&Documento=Informe&Proceso=CrearSeguimientoObservacion'> Informe de Observaci&oacute;n</a></p>";

    //bodyEmail += "</div></td></tr></tbody></table></td></tr></tbody></table></td>";
    //bodyEmail += "</tr>";
    //bodyEmail += "<tr>";
    //bodyEmail += "<td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' style='border-top:1px solid #ccc;' width='100%'><tbody><tr><td valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr>";
    //bodyEmail += "<td colspan='2' style='padding-top: 30px;border:0;color:#acacac;font-family:Arial;font-size:12px;line-height:125%;text-align:left' valign='middle'>�2020 Visitasa. Todos los derechos reservados</td></tr></tbody></table></td></tr></tbody></table></td>";
    //bodyEmail += "</tr>";
    //bodyEmail += "</tbody></table></body>";

    var bodyEmail = "";
    bodyEmail += "<p></p>";
    bodyEmail += "<p>Estimados,</p>";
    bodyEmail += "<p></p>";
    bodyEmail += "<p>Se ha reportado una observaci&oacute;n preventiva - SALVA, con los siguientes datos:</p>";
    bodyEmail += "<p></p>";

    bodyEmail += "<table style='width: 600px; border-collapse: collapse;'>";
    bodyEmail += "<tbody>";

    bodyEmail += "<tr>";
    bodyEmail += "<td style='background: #fabf8f; padding: 5px; border: 1px solid #000; text-align: left; font-size: 14px;'>Nro. de Reporte</ td>";
    bodyEmail += "<td style='text-align: center; padding: 5px; border: 1px solid #000; font-size: 14px;'>" + obj.Codigo + "</ td>";
    bodyEmail += "</tr>";

    bodyEmail += "<tr>";
    bodyEmail += "<td style='background: #fabf8f; padding: 5px; border: 1px solid #000; text-align: left; font-size: 14px;'>Tipo de Observaci&oacute;n preventiva</ td>";
    bodyEmail += "<td style='text-align: center; padding: 5px; border: 1px solid #000; font-size: 14px;'>" + ent.Tipo_Observacion_Des + "</ td>";
    bodyEmail += "</tr>";

    bodyEmail += "<tr>";
    bodyEmail += "<td style='background: #fabf8f; padding: 5px; border: 1px solid #000; text-align: left; font-size: 14px;'>Fecha</ td>";
    bodyEmail += "<td style='text-align: center; padding: 5px; border: 1px solid #000; font-size: 14px;'>" + ent.Fecha_Operacion.ToString("yyyy-MM-dd") + "</ td>";
    bodyEmail += "</tr>";

    bodyEmail += "<tr>";
    bodyEmail += "<td style='background: #fabf8f; padding: 5px; border: 1px solid #000; text-align: left; font-size: 14px;'>Hora</ td>";
    bodyEmail += "<td style='text-align: center; padding: 5px; border: 1px solid #000; font-size: 14px;'>" + ent.Hora_Operacion + "</ td>";
    bodyEmail += "</tr>";

    bodyEmail += "<tr>";
    bodyEmail += "<td style='background: #fabf8f; padding: 5px; border: 1px solid #000; text-align: left; font-size: 14px;'>Sede/Embarcaci&oacute;n</ td>";
    bodyEmail += "<td style='text-align: center; padding: 5px; border: 1px solid #000; font-size: 14px;'>" + ent.Sede_Des + ent.Embarcacion_Des + "</ td>";
    bodyEmail += "</tr>";

    bodyEmail += "<tr>";
    bodyEmail += "<td style='background: #fabf8f; padding: 5px; border: 1px solid #000; text-align: left; font-size: 14px;'>&Aacute;rea responsable de la correcci&oacute;n o cierre</ td>";
    bodyEmail += "<td style='text-align: center; padding: 5px; border: 1px solid #000; font-size: 14px;'>" + ent.Area_Des + "</ td>";
    bodyEmail += "</tr>";

    bodyEmail += "<tr>";
    bodyEmail += "<td style='background: #fabf8f; padding: 5px; border: 1px solid #000; text-align: left; font-size: 14px;'>EP</ td>";
    bodyEmail += "<td style='text-align: center; padding: 5px; border: 1px solid #000; font-size: 14px;'> - </ td>";
    bodyEmail += "</tr>";

    bodyEmail += "<tr>";
    bodyEmail += "<td style='background: #fabf8f; padding: 5px; border: 1px solid #000; text-align: left; font-size: 14px;'>Reportante (Nombre y Apellidos)</ td>";
    bodyEmail += "<td style='text-align: center; padding: 5px; border: 1px solid #000; font-size: 14px;'>" + obj.Nombres_Reportante + "</ td>";
    bodyEmail += "</tr>";

    bodyEmail += "<tr>";
    bodyEmail += "<td style='background: #fabf8f; padding: 5px; border: 1px solid #000; text-align: left; font-size: 14px;'>Zona/Equipo</ td>";
    bodyEmail += "<td style='text-align: center; padding: 5px; border: 1px solid #000; font-size: 14px;'>" + ent.Zona_Des + "</ td>";
    bodyEmail += "</tr>";

    //bodyEmail += "<tr>";
    //bodyEmail += "<td style='background: #fabf8f;'>Observaci�n</ td>";
    //bodyEmail += "<td style='text-align: center;'> Matman </ td>";
    //bodyEmail += "</tr>";

    //bodyEmail += "<tr>";
    //bodyEmail += "<td style='background: #fabf8f;'>A que se debe comportamiento inseguro</ td>";
    //bodyEmail += "<td style='text-align: center;'> Matman </ td>";
    //bodyEmail += "</tr>";

    //bodyEmail += "<tr>";
    //bodyEmail += "<td style='background: #fabf8f;'>Procedimiento que incumple</ td>";
    //bodyEmail += "<td style='text-align: center;'> Matman </ td>";
    //bodyEmail += "</tr>";

    bodyEmail += "<tr>";
    bodyEmail += "<td style='background: #fabf8f; padding: 5px; border: 1px solid #000; text-align: left; font-size: 14px;'>Detalle</ td>";
    bodyEmail += "<td style='text-align: center; padding: 5px; border: 1px solid #000; font-size: 14px;'>Click aqu&iacute; (<a href='https://www.visualsatpe.com/beta/tasassoma/view/download.html?Id=" + obj.Id + "&Documento=Informe&Proceso=CrearSeguimientoObservacion'>enlace para abrir la OP</a>)</ td>";
    bodyEmail += "</tr>";

    bodyEmail += "</ tbody>";
    bodyEmail += "</ table>";


    oDataEmail.bodyhtml = bodyEmail;


    // ENVIO DEL EMAIL
    var jsonuser3 = JsonConvert.SerializeObject(oDataEmail);
    var datauser3 = new StringContent(jsonuser3, Encoding.UTF8, "application/json");
    var urlemail3 = "https://7454em4ils3nder-app.azurewebsites.net/api/VisitasaSendEmail?code=LXQwITmvDAAqXTgaDcBAkmbZXBCv5KnS6bY/XszaOjqHus4M3dbDzw==";
    httpclient.DefaultRequestHeaders.Add("apikey", "r$3#23516ewew5");
    var responseuser3 = await httpclient.PostAsync(urlemail3, datauser3);
    string resultEmail = responseuser3.Content.ReadAsStringAsync().Result;

    log.LogInformation("resultEmail " + resultEmail);
}

public class DataEmailNew
{
    public string sendto { get; set; }
    public string from { get; set; }
    public string emailsubject { get; set; }
    public string bodyhtml { get; set; }
    public List<Files> Attachments { get; set; }
}

public class Files
{
    public string base64 { get; set; }
    public string name { get; set; }
}

public class Account
{
    public int error { get; set; }
    public string message { get; set; }
}




//////////////////////////////// PRUEBA ENVIO DE CORREO **********************************
// public class PushSendEmail
// {           
    public static Account sendEmail(ILogger log, DataEmailNew dEmail)
    {        
        Account oAccount = new Account();
        log.LogInformation("try");  
        try
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            //string To          = "erivas@visualsat.com";
            //dEmail.From
            log.LogInformation("dEmail.Attachments");  
            if(dEmail.Attachments!=null){
                foreach(var attachment in dEmail.Attachments){
                    var bytes = Convert.FromBase64String(attachment.base64);
                    MemoryStream strm = new MemoryStream(bytes);
                    Attachment data = new Attachment(strm, attachment.name);
                    mail.Attachments.Add(data);                     
                }
            }
            log.LogInformation("mail.From");  
            mail.From          = new MailAddress("visualsatuser@tasa.com.pe");
            log.LogInformation("mail.To");  
            mail.To.Add(new MailAddress(dEmail.sendto));
            log.LogInformation("mail.Subject");  
            mail.Subject       = dEmail.emailsubject;
            log.LogInformation("mail.Body");  
            mail.Body          = dEmail.bodyhtml;
            log.LogInformation("mail.IsBodyHtml");  
            mail.IsBodyHtml    = true;
            log.LogInformation("SmtpClient");  
            var client          = new SmtpClient();
            /*client.Host         = "smtp.gmail.com";
            client.Port         = 587;
            client.Credentials  = new NetworkCredential("rivasmej@gmail.com","xxxxxxxxxxxxxxxxx");
            client.EnableSsl    = true;*/
            client.Host         = "smtp.office365.com";
            client.Port         = 587;
            client.Credentials  = new NetworkCredential("visitasa@tasa.com.pe","V1s174sa000779");  
            //client.Credentials  = new NetworkCredential("visualsatuser@tasa.com.pe","xxxxxxxxxxxxx");                         
            client.UseDefaultCredentials = false; 
            client.EnableSsl    = false;
            client.TargetName = "STARTTLS/smtp.office365.com";
            // Send it...       
            log.LogInformation("client.Send(mail)");    
            client.Send(mail);
            log.LogInformation("Email sccessfully sent");            
            oAccount.error=0;
            oAccount.message="Email sccessfully sent";
            return oAccount;
        }
        catch (Exception ex)
        {
            log.LogInformation("Error in sending email: "+ ex.Message);
            //Console.WriteLine("Error in sending email: " + ex.Message);
            //Console.ReadKey();            
            oAccount.error=1;
            oAccount.message=ex.Message;
            return oAccount;
        }
    }
    
// }
//////////////////////////////// PRUEBA ENVIO DE CORREO **********************************

