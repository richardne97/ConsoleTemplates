certutil -f -p %1 -importpfx "Server.pfx"
netsh http add sslcert ipport=0.0.0.0:7800 appid={f5bb1630-a697-4d3c-87ac-ed91199957a5} certhash=3919c8234bfa28bc844f6c152550b6351c55b345
pause