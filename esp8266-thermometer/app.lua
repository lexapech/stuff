collectgarbage()  

temp=0
trig1=0
trig2=0
state1=0
state2=0
time=0
ow.setup(4)
gpio.mode(1,gpio.OUTPUT)
gpio.mode(2,gpio.OUTPUT)

if file.open("tres.txt") then
    _ , _ , d1,d2 = string.find(file.read(),"(.+) (.+)") 
    file.close()
    if tonumber(d1)~=nil then
        trig1=tonumber(d1)
    end
    if tonumber(d2)~=nil then
        trig2=tonumber(d2)
    end    
end


tim=tmr.create()
tim:alarm(60000,tmr.ALARM_AUTO,function()
    time=time+1
    if time>710 then
        node.restart() 
    end
    data=0
    print("conv start")
    ow.reset(4)
    ow.skip(4)
    ow.write(4,0x44,1)  
    local mytimer = tmr.create()
    mytimer:register(2000, tmr.ALARM_SINGLE, function (t)  
        print("reading temp")      
        ow.reset(4)
        ow.skip(4)
        ow.write(4,0xBE,1)
        data=(ow.read(4)+ow.read(4)*256)*625
        temp=data/10000
        t:unregister() 
        print(temp)
        if temp > trig1 then
            gpio.write(1,gpio.LOW)
            state1=0
        elseif temp < trig1 then
            gpio.write(1,gpio.HIGH)
            state1=1
        end
        if temp > trig2 then
            gpio.write(2,gpio.LOW)
            state2=0
        elseif temp < trig2 then
            gpio.write(2,gpio.HIGH)
            state2=1
        end
    end) 
    mytimer:start() 
end)

send = function(cl)   
    local str=file.read()
    if(str~=nil) then
        cl:send(str,send)
    else
        file.close()
        cl:close()
    end
end 

srv=net.createServer(net.TCP,2)   
    srv:listen(80,function(conn)   
        collectgarbage()      
        conn:on("receive",function(client,request)   

           
            local _ , _ , method,path = string.find(request,"([A-Z]+) (.+) HTTP")
            print(request)          
            if method=="GET" then
                local _ , _ , action,name = string.find(path,"/(.+):(.+)")
                if path=="/" then
                    client:send("HTTP/1.1 303 OK\nLocation: /index.html\n\n")
                elseif path=="/reboot" then
                    client:send("HTTP/1.1 303 OK\nLocation: /update.html\n\n",function(c)
                        srv:close()
                        createserv()
                    end)  
                elseif path=="/data" then
                    buf=temp..";"..state1..";"..state2..";"..trig1..";"..trig2..";"..time                 
                    client:send("HTTP/1.1 200 OK\nConnection: close\nContent-Type: text/plain\nContent-Length: "..string.len(buf).."\n\n"..buf)                                 
                else
                    local _ , _ , path = string.find(request,"/(.+) HTTP")
                    if file.open(path, "r") then
                        client:send("HTTP/1.1 200 OK\nConnection: close\nContent-Type: text/html\nContent-Length: "..file.stat(path).size.."\n\n"..file.read(),send)
                    else 
                        client:send("HTTP/1.1 404 NotFound\nConnection: close\nContent-Length: 13\n\n404 NOT FOUND")
                    end
                end
            elseif method=="POST" then
                    if path=="/trig" then
                        local _ , _ , t1,t2 = string.find(request,"1:(.+) 2:(.+)")
                        file.open("tres.txt","w+")
                        file.write(t1.." "..t2)
                        file.close()
                        trig1=tonumber(t1)
                        trig2=tonumber(t2)
                        client:send("HTTP/1.1 200 OK\nConnection: close\nContent-Type: text/html\nContent-Length:0\n\n\n")
                    else
                        client:send("HTTP/1.1 404 NotFound\nConnection: close\nContent-Length: 13\n\n404 NOT FOUND")
                    end
            end          
        end)

        conn:on("disconnection",function(client)  
            collectgarbage()       
        end)
    end)






    



