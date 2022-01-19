importClass(
    java.net.ServerSocket,
    java.io.BufferedReader,
    java.io.InputStreamReader,
    java.io.OutputStream,
);

function ditalks(replier){
    try {
        //소켓 생성
        const socket = new java.net.Socket("ip", 8080);
        socket.setSoTimeout(15000)
        Log.d(1)
        const input = socket.getInputStream();
        let line = new java.nio.ByteBuffer.allocate(1024).array();
        while(1) {
          try {
              let lcnt = input.read(line)
              let message = JSON.parse(UTF8stringFromJavaByteArray(line,lcnt))
              let msg = message.Message;
              let sender = message.SendBy
              if(msg=="exit") break;
              replier.reply(sender + " : " + msg)
          } catch (e) {
            
          }
        }
        socket.close();
    } catch (e) {
        Log.e(e);
    }
}

function response(room, msg, sender, isGroupChat, replier, imageDB, packageName) {
    if(msg == "소켓 테스트"&&room=="IROOM") {
        ditalks(replier)
    }
}

function UTF8stringFromJavaByteArray(line,lcnt)
    {
        let data = []
        for(i=0;i<lcnt;i++) data[i] = line[i];
        const extraByteMap = [ 1, 1, 1, 1, 2, 2, 3, 0 ];
        var count = data.length;
        var str = "";
        for (var index = 0;index < count;)
        {
            var ch = data[index++];
            if (ch & 0x80)
            {
                var extra = extraByteMap[(ch >> 3) & 0x07];
                if (!(ch & 0x40) || !extra || ((index + extra) > count))
                    return null;
                ch = ch & (0x3F >> extra);
                for (;extra > 0;extra -= 1)
                {
                    var chx = data[index++];
                    if ((chx & 0xC0) != 0x80)
                        return null;
                    ch = (ch << 6) | (chx & 0x3F);
                }
            }
            str += String.fromCharCode(ch);
        }
        return str;
    }
