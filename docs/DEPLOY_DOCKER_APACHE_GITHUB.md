# Deploy với Docker + Apache qua GitHub Actions

Tài liệu này dùng cho flow deploy tự động từ `.github/workflows/api-ci-cd.yml`.

## 1) Mô hình triển khai

- API + PostgreSQL chạy bằng Docker Compose trên server
- Apache chạy trên host server để reverse proxy vào API container
- GitHub Actions build/push image và SSH vào server để deploy

## 2) GitHub Secrets cần tạo

Vào: **Repo → Settings → Secrets and variables → Actions → New repository secret**

### SSH / host
- `API_DEPLOY_HOST` (VD: `1.2.3.4`)
- `API_DEPLOY_PORT` (VD: `22`)
- `API_DEPLOY_USER` (VD: `deploy`)
- `API_DEPLOY_SSH_KEY` (private key PEM)
- `API_DEPLOY_PATH` (VD: `/home/deploy/cleanarchitecture-api`)

### Docker image/deploy
- `API_DOCKER_IMAGE` (VD: `ghcr.io/<owner>/<repo>/cleanarchitecture-api:develop`)
- `API_DBMIGRATOR_IMAGE` (VD: `ghcr.io/<owner>/<repo>/cleanarchitecture-dbmigrator:develop`)
- `API_DOCKER_CONTAINER` (VD: `cleanarchitecture-api`)
- `API_DOCKER_HOST_PORT` (VD: `5055`)

### Registry (nếu pull private GHCR)
- `API_REGISTRY_USERNAME`
- `API_REGISTRY_TOKEN` (PAT có `read:packages`)

### App settings (bắt buộc production)
- `API_POSTGRES_DB` (VD: `CleanArchitecture`)
- `API_POSTGRES_USER` (VD: `postgres`)
- `API_POSTGRES_PASSWORD` (mật khẩu PostgreSQL production)
- `API_JWT_SECRET_KEY` (>= 32 ký tự)
- `API_JWT_ISSUER` (VD: `cleanarchitecture-api`)
- `API_JWT_AUDIENCE` (VD: `cleanarchitecture-clients`)
- `API_ALLOWED_ORIGINS` (VD: `https://app.example.com`)

### App settings (khuyến nghị)
- `API_ALLOW_CREDENTIALS` (`true` hoặc `false`, mặc định `false`)
- `API_KNOWN_PROXIES` (VD: `127.0.0.1` khi Apache cùng host)

---

## 3) Apache reverse proxy cấu hình

Tạo file `/etc/apache2/sites-available/cleanarchitecture-api.conf`:

```apache
<VirtualHost *:80>
    ServerName api.example.com

    ProxyPreserveHost On
    ProxyRequests Off
    RequestHeader set X-Forwarded-Proto "http"

    ProxyPass / http://127.0.0.1:5055/
    ProxyPassReverse / http://127.0.0.1:5055/

    ErrorLog ${APACHE_LOG_DIR}/cleanarchitecture-api-error.log
    CustomLog ${APACHE_LOG_DIR}/cleanarchitecture-api-access.log combined
</VirtualHost>
```

Enable modules/site:

```bash
sudo a2enmod proxy proxy_http headers rewrite
sudo a2ensite cleanarchitecture-api.conf
sudo systemctl reload apache2
```

### SSL (khuyến nghị)

```bash
sudo apt update
sudo apt install certbot python3-certbot-apache -y
sudo certbot --apache -d api.example.com
```

---

## 4) Verify sau deploy

- `https://api.example.com/health/live`
- `https://api.example.com/health/ready`

Nếu `/health/ready` fail, kiểm tra PostgreSQL container và log migrator.

---

## 5) Ghi chú

- Workflow hiện deploy khi push `develop`.
- Compose deploy file được sinh động trên server từ secrets.
- Nếu muốn nhiều AllowedOrigins, mở rộng workflow để map thêm `ApiSecurity__AllowedOrigins__1`, `__2`, ...
