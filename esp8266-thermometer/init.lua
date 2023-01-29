login=""
password=""
disconnect=0
maxdisc=10
errmsg="no errors"
function startup()
    if file.exists("app.lua") then      
       print("starting app")          
       ok,errmsg=pcall(dofile,"app.lua")
       if ok==false then
            print(errmsg) 
            createserv() 
        end      
    else
    print("no app")
    print("starting server")
    createserv()
    end
end

send = function(cl)   
    local str=file.read()
    if(str~=nil) then
        cl:send(str,send)
    else
        file.close()
        cl:close()
    end
end 

function createserv()
    collectgarbage()  
    srv=net.createServer(net.TCP,2)   
    srv:listen(80,function(conn)   
        collectgarbage()      
        conn:on("receive",function(client,request)   

            
            local _ , _ , method,path = string.find(request,"([A-Z]+) (.+) HTTP")         
            if method=="GET" then
                local _ , _ , action,name = string.find(path,"/(.+):(.+)")
                if path=="/" then
                    client:send("HTTP/1.1 303 OK\nLocation: /update.html\n\n")
                elseif path=="/reboot" then
                    client:send("HTTP/1.1 303 OK\nLocation: /\n\n",function(c)
                        c:close() 
                        node.restart() 
                    end)                    
                elseif action=="remove" then
                    if name~="init.lua" then
                        file.remove(name)
                        client:send("HTTP/1.1 200 OK\nConnection: close\nContent-Type: text/html\nContent-Length:0\n\n\n")
                    else
                        client:send("HTTP/1.1 401 Denied\nConnection: close\nContent-Type: text/html\nContent-Length:0\n\n\n")
                    end
                elseif path=="/files" then
                    l=file.list()
                    local buf=""
                    for k,v in pairs(l) do
                         buf=buf.."<p><a href=\"/"..k.."\">"..k.." : "..v.."  </a><a  href=\"/remove:"..k.."\">[X]</a></p>\n"
                    end
                    local _, used, total=file.fsinfo()
                    buf=buf.."<p>"..(used/1024).." / "..(total/1024).." kB</p>"
                    buf=buf.."<p>RAM "..collectgarbage("count").." kB</p>"
                    if errmsg~=nil then
                    buf=buf.."<p>"..errmsg.."</p>"
                    end
                    client:send("HTTP/1.1 200 OK\nConnection: close\nContent-Type: text/html\nContent-Length: "..string.len(buf).."\n\n"..buf)
                else
                    local _ , _ , path = string.find(request,"/(.+) HTTP")
                    if file.open(path, "r") then
                        client:send("HTTP/1.1 200 OK\nConnection: close\nContent-Type: text/html\nContent-Length: "..file.stat(path).size.."\n\n"..file.read(),send)
                    else 
                        client:send("HTTP/1.1 404 NotFound\nConnection: close\nContent-Length: 13\n\n404 NOT FOUND")
                    end
                end
            elseif method=="POST" then
                local _ , _ , name = string.find(request,"name:\n(.+)\nendname\n")
                local _ , _ , data = string.find(request,"content:\n(.+)")
                print(name)
                print(data)                
                if name ~= nil and data ~= nil then  
                if path=="/first" then
                    file.remove(name)    
                end   
                    file.open(name,"a+")
                    file.write(data)
                    file.close()
                end
                client:send("HTTP/1.1 200 OK\nConnection: close\nContent-Type: text/html\nContent-Length:0\n\n\n")
            end
        end)

        conn:on("disconnection",function(client)  
              collectgarbage()        
        end)
    end)
end

onconnect = function(T)
    print("connecting")
    disconnect=0
end

onip = function(T)
    print(T.IP)
    startup()
end

ondisconnect = function(T)
    if T.reason == wifi.eventmon.reason.ASSOC_LEAVE then
        return
    end
    disconnect = disconnect + 1
    print("disconnected "..disconnect)
    print(T.reason)
    if disconnect>maxdisc then
        wifi.sta.disconnect()
        ap()
    end
end

function ap()
    wifi.setmode(wifi.SOFTAP)
    wifi.ap.config({ssid="esp82",pwd="12345678"})
    wifi.ap.dhcp.start()
    startup()
end
collectgarbage()
if file.open("user.txt") then
    login=string.sub(file.read(";"),0,-2)
    password=file.read(";")
    print("read user")
    print(login.." "..password)
    wifi.eventmon.register(wifi.eventmon.STA_CONNECTED, onconnect)
    wifi.eventmon.register(wifi.eventmon.STA_GOT_IP, onip)
    wifi.eventmon.register(wifi.eventmon.STA_DISCONNECTED, ondisconnect)

    wifi.setmode(wifi.STATION)
    wifi.sta.config({ssid=login,pwd=password})
else
    ap()
end


