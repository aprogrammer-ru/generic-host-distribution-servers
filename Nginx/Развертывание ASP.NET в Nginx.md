### Базовая установка Nginx, ASP.NET Runtime, настройке демона и запуск приложения ASP.NET в Debian

#### 1. Установка Nginx

1. Обновите пакеты:

   `sudo apt update && sudo apt install nginx -y`

2. Установите Nginx:

   `sudo apt install nginx -y`

3. Запустите и добавьте Nginx в автозагрузку:

    ```
    sudo systemctl start nginx
    sudo systemctl enable nginx
	```

4. Проверьте статус Nginx:

   `sudo systemctl status nginx`

#### 2. Установка ASP.NET Runtime

1. Добавьте репозиторий Microsoft:

    ```
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
	```

2. Установите ASP.NET Runtime:

    ```
    sudo apt update
    sudo apt install aspnetcore-runtime-9.0 -y
    ```
   (Замените `9.0` на нужную версию, если требуется.)


#### 3. Настройка демон-процесса для приложения ASP.NET

1. Создайте файл службы для вашего приложения:

   `sudo nano /etc/systemd/system/demoapi.service`

2. Добавьте следующий конфигурационный файл:

```
[Unit]
Description=Demo ASP.NET Application

[Service]
WorkingDirectory=/home/dev-user/api
ExecStart=/usr/bin/dotnet web-service-demo.dll
Restart=always
RestartSec=10
SyslogIdentifier=demoapi
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```
   (Замените `/home/dev-user/api/web-service-demo.dll` на путь к вашему приложению.)

3. Перезагрузите демон systemd:

   `sudo systemctl daemon-reload`

4. Запустите и добавьте службу в автозагрузку:

    ```
    sudo systemctl start demoapi
    sudo systemctl enable demoapi
	```

5. Проверьте статус службы:

   `sudo systemctl status demoapi`


#### 4. Настройка Nginx для проксирования запросов к ASP.NET приложению

6. Откройте конфигурационный файл Nginx:

   `sudo nano /etc/nginx/sites-available/demoapi`

7. Добавьте следующий конфигурационный файл:

    ```   
    server {
        listen 8077;
        server_name localhost;
    
        location / {
            proxy_pass         http://localhost:5000;
            proxy_http_version 1.1;
            proxy_set_header   Upgrade $http_upgrade;
            proxy_set_header   Connection keep-alive;
            proxy_set_header   Host $host;
            proxy_cache_bypass $http_upgrade;
            proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header   X-Forwarded-Proto $scheme;
        }
    }
	```

   (Замените `localhost` на ваш домен или IP-адрес.)

8. Создайте символическую ссылку и перезагрузите Nginx:

    ```
    sudo ln -s /etc/nginx/sites-available/demoapi /etc/nginx/sites-enabled/
    sudo nginx -t
    sudo systemctl reload nginx
    ```

#### 5. Запуск приложения ASP.NET

1. Убедитесь, что файлы приложения скопированы в директорию `/home/dev-user/api/`.

2. Проверьте, что служба `demoapi` работает:

   `sudo systemctl status demoapi`

3. Откройте браузер и перейдите по вашему домену или IP-адресу (в примере `http://localhost:8077`). Ваше приложение должно быть доступно.


Готово! Ваше ASP.NET приложение теперь работает за Reverse-Proxy Nginx.