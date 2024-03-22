##########################
__author__="Guru"
#
#Parameters_required = NONE
#
##########################

import paramiko
import sys,os
import json
import requests
from paepy.ChannelDefinition import CustomSensorResult
from html_table_parser import HTMLTableParser

try:
    data = json.loads(sys.argv[1])
except:
    data = {"host":"172.25.200.161", "linuxloginusername":"tejas", "linuxloginpassword":"j72e#05t", "params":"TJ1418"}

ip=data['host']
username = data.get('linuxloginusername', 'tejas')
passwd = data.get('linuxloginpassword', 'j72e#05t')
port = 22

def NeSession():
    try:
        session = requests.Session()
        session.auth = (username, passwd)
        session.headers.update({"Cookie":"LOGIN_LEVEL=2; path=/;"})
        return session
    except Exception as e:
        print (e)

def NeGetObject(ip,ObjectName):
    try:
        s = NeSession()
        try:
            url = "http://"+ip+":20080/NMSRequest/GetObjects?NoHTML=true&Objects="+str(ObjectName)
            re = s.get(url)
        except:
            url = "https://"+ip+"/NMSRequest/GetObjects?NoHTML=true&Objects="+str(ObjectName)
            re = s.get(url, verify=False)
        info = {}
        infoArr = re.text.strip().split("\t")
        info.update({'ObjectName' : infoArr[0]})
        for i in range(2, len(infoArr[2:]), 2):
            info.update({infoArr[i][1:] : infoArr[i+1]})
            
        return info
    except Exception as e:
        print(e)
        return False


def NeGetObjects(ip,Objects):
    try:
        s=NeSession()
        try:
            url = "http://"+ip+":20080/NMSRequest/GetObjects?NoHTML=true&Objects="+Objects
            re = s.get(url)
        except:
            url = "https://"+ip+"/NMSRequest/GetObjects?NoHTML=true&Objects="+Objects
            re = s.get(url, verify=False)
        data = re.text.strip()
        if data.find('no objects') != -1:
            return False
        ObjectArr = data.split('\n')
        ObjectList = []
        for i in ObjectArr:
            ObjectList.append(i.strip().split('\t')[0])

        return ObjectList
    except Exception as e:
        print(e)
        return False
    
def execute_ssh(command, raw=0):
    _,stdout,_=ssh.exec_command(command)
    data = stdout.read().decode('utf-8')
    if raw:
        return data
    return data.strip().splitlines()

#ssh to NE
ssh=paramiko.SSHClient()
ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
ssh.connect(ip,port,username,passwd)

topdata=execute_ssh("top -bn1")

meminfo=execute_ssh("cat /proc/meminfo ")

msd_test=execute_ssh("cat /proc/msd_test")

smart=execute_ssh("cat /proc/tejas/storage/smart/smartInfo")

ssh.close()

nm = pm = cc = fm = ospf = gpon = gmpls = chAgent = net = SvcSwitchMgr = l2SvcMgr = bcmHal = ifmgr = ifagent = ssmtm = {}

memplace = 6
cpuplace = -3
system = NeGetObject(ip,"System-1")

if system.get('ProductCode', 'NA') == "TJMC140018":
    cpuplace = -4
uptime = system.get('UpTime', 0)

for d in topdata:
    if 'nm.d' in d:
        nm ={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}
        

    if 'pm.d' in d:
        pm ={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}


    if 'cc.d' in d:
        cc ={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}


    if 'fm.d' in d:
        fm ={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}

    if 'ospf.d' in d:
        ospf={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}

    if 'gpon.d' in d:
        gpon={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}
    
    if 'gmpls.d' in d:
        gmpls={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}
    
    if 'chAgent.d' in d:
        chAgent={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}
    
    if 'net.d' in d:
        net={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}
    
    if 'SvcSwitchMgr.d' in d:
        SvcSwitchMgr={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}
    
    if 'l2SvcMgr.d' in d:
        l2SvcMgr={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}
    
    if 'bcmHal.d' in d:
        bcmHal={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}
 
    if 'ifmgr.d' in d:
        ifmgr={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}
    
    if 'ifagent.d' in d:
        ifagent={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}
    
    if 'ssmtm.d' in d:
        ssmtm={'cpu':d.split()[cpuplace], 'mem':d.split()[memplace]}
    


    if 'Load' in d:
        Load ={'cpu':d.split()[2]}
    
Tmem = 0
Fmem = 0
for m in meminfo:
    if 'MemTotal' in m:
        Tmem = m.split()[1]
    if 'MemFree' in m:
        Fmem = m.split()[1]


#print(nm)
# create sensor result
result = CustomSensorResult("NE health monitoring on: {0} RAM: {1:.2f} MB Uptime: {2}Hrs {3}Min".format(ip,int(Tmem)/1024, int(uptime)//3600, (int(uptime)%3600)//60 ))

# add primary channel
result.add_channel(channel_name="CPU load", unit=" ", value=Load['cpu'], is_float=True, primary_channel=True,
                       is_limit_mode=True, limit_min_error=0, limit_max_error=10,
                       limit_error_msg="Percentage too high", decimal_mode='Auto')

result.add_channel(channel_name="UsedMemory", unit="MB", value=(int(Tmem)-int(Fmem))/1024, is_float=False, decimal_mode='Auto')
# add additional channel
result.add_channel(channel_name="CPU_NM.d", unit="%", value=nm.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_PM.d", unit="%", value=pm.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_FM.d", unit="%", value=fm.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_CC.d", unit="%", value=cc.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_ospf.d", unit="%", value=ospf.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_gpon.d", unit="%", value=gpon.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_gmpls.d", unit="%", value=gmpls.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_net.d", unit="%", value=net.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_SvcSwitchMgr.d", unit="%", value=SvcSwitchMgr.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_l2SvcMgr.d", unit="%", value=l2SvcMgr.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_bcmHal.d", unit="%", value=bcmHal.get('cpu',0), is_float=True, decimal_mode='Auto')

result.add_channel(channel_name="CPU_ifmgr.d", unit="%", value=ifmgr.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_ifagent.d", unit="%", value=ifagent.get('cpu',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="CPU_ssmtm.d", unit="%", value=ssmtm.get('cpu',0), is_float=True, decimal_mode='Auto')


result.add_channel(channel_name="MEM_NM.d", unit="%", value=nm.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_PM.d", unit="%", value=pm.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_FM.d", unit="%", value=fm.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_CC.d", unit="%", value=cc.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_ospf.d", unit="%", value=ospf.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_gpon.d", unit="%", value=gpon.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_gmpls.d", unit="%", value=gmpls.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_net.d", unit="%", value=net.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_SvcSwitchMgr.d", unit="%", value=SvcSwitchMgr.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_l2SvcMgr.d", unit="%", value=l2SvcMgr.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_bcmHal.d", unit="%", value=bcmHal.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_ifmgr.d", unit="%", value=ifmgr.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_ifagent.d", unit="%", value=ifagent.get('mem',0), is_float=True, decimal_mode='Auto')
result.add_channel(channel_name="MEM_ssmtm.d", unit="%", value=ssmtm.get('mem',0), is_float=True, decimal_mode='Auto')





def main(ip):
    s=NeSession()
    try:
            url = "http://"+ip+":20080/EMSRequest/VoltageStatistics"
            re = s.get(url)
    except:
            url = "https://"+ip+"/EMSRequest/VoltageStatistics"
            re = s.get(url, verify=False)
    xhtml = re.text.strip()

    p = HTMLTableParser()
    p.feed(xhtml)
    return p.tables


a = main(ip)
psus = []
for i in range(1,len(a[0])):
    psus.append(dict(zip(a[0][0],a[0][i])))

    

for psu in psus:
    result.add_channel(channel_name=psu.get("Card Name"), unit="V", value=psu.get("Current Voltage Value (Volts)"), is_float=True, decimal_mode='Auto')

objs = NeGetObjects(ip,'Card')

for obj in objs:
    temp = NeGetObject(ip,obj)
    if temp:
        if int(temp['Temp']) > 0:
            result.add_channel(channel_name=temp['ObjectName'], unit="Temperature", value=temp['Temp'], is_float=True, decimal_mode='Auto')

PCycle=0
Life=0

for i in smart:
    if "Remaining Life" in i:
        Life = i.split("=")[1].strip()
    if "Power Cycle" in i:
        PCycle = i.split("=")[1].strip()

if Life:
    result.add_channel(channel_name="Remaining Life", unit="Percent", value=Life)
    result.add_channel(channel_name="Power Cycle", value=PCycle)


# print sensor result to stdout
print(result.get_json_result())
