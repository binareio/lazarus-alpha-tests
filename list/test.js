function writeCommentDate(comment, dateDiv)
	{
		dateDiv.innerText = '';
		var ts = new Date(comment.modifiedDate);
		var str = editorUi.timeSince(ts);
		if (str == null)
		{
			str = mxResources.get('lessThanAMinute');
		}
		mxUtils.write(dateDiv, mxResources.get('timeAgo', [str], '{1} ago'));
		dateDiv.setAttribute('title', ts.toLocaleDateString() + ' ' +
				ts.toLocaleTimeString());
	};


function embedVimeoIframe(e){
		var elem = e.currentTarget;
		var id = elem.getAttribute('data-vimeo');
		var vimeoParams = elem.getAttribute('data-vimeoparams') || '';
		elem.removeEventListener('click', embedVimeoIframe);
		if (!id || !regValidParam.test(id) || (vimeoParams && !regValidParam.test(vimeoParams))) {
			return;
		}
		if(vimeoParams && !regAmp.test(vimeoParams)){
			vimeoParams = '&'+ vimeoParams;
		}
		e.preventDefault();
		elem.innerHTML = '<iframe src="' + (vimeoIframe.replace(regId, id)) + vimeoParams +'" ' +
			'frameborder="0" allowfullscreen="" width="640" height="390"></iframe>'
		;
	}


function isArrayBuffer(obj) {
	return toString.call(obj) === "[object ArrayBuffer]";
}

function appendline(session, text)
	{
		if(chats[session] && chats[session].content)
		{
			text = AjaxLife.Utils.LinkURLs(text.escapeHTML());
			var line = Ext.get(document.createElement('div'));
			line.addClass(["agentmessage","chatline"]);
			var timestamp = Ext.get(document.createElement('span'));
			timestamp.addClass("chattimestamp");
			var time = new Date();
			timestamp.dom.appendChild(document.createTextNode("["+time.getHours()+":"+((time.getMinutes()<10)?("0"+time.getMinutes()):time.getMinutes())+"]"));
			line.dom.appendChild(timestamp.dom);
			line.dom.appendChild(document.createTextNode(" "));
			var span = document.createElement('span');
			span.innerHTML = text;
			line.dom.appendChild(span);
			chats[session].content.dom.appendChild(line.dom);
			chats[session].content.dom.scrollTop = chats[session].content.dom.scrollHeight;
		}
		else
		{
			AjaxLife.Widgets.Ext.msg("Warning","Instant message with unknown ID {0}:<br />{1}",session,text);
		}
	};

Editor.selectFilename = function(b) {
    var e = b.value.lastIndexOf(".");
    if (0 < e) {
        var g = b.value.substring(e + 1);
        "drawio" != g && 0 <= mxUtils.indexOf(["png", "svg", "html", "xml"], g) && (g = b.value.lastIndexOf(".drawio.", e), 0 < g && (e = g))
    }
    e = 0 < e ? e : b.value.length;
    Editor.selectSubstring(b, 0, e);
};

function selectedFilesRail(inputFileID) {
    var fileobj = [];
    if (inputFileID && inputFileID != '') {
        setFilesRail(document.getElementById(inputFileID).files);
    }
}

Git.prototype.binaryCatFile = function() {
   return this._catFile('buffer', arguments);
};
