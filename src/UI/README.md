# CleanArchitecture UI

Frontend app built with React + TypeScript + Vite + Tailwind.

## Prerequisites

- Node.js 20+
- npm 10+

## Environment

Create `src/UI/.env` from `src/UI/.env.example` and set:

```bash
VITE_API_BASE_URL=https://your-api-url
```

## Run locally

```bash
cd src/UI
npm ci
npm run dev
```

## Quality checks

```bash
npm run lint
npm run typecheck
npm run build
```

## CI/CD

Workflow file: `.github/workflows/ui-ci.yml`

### CI

Runs on:

- Pull requests that touch `src/UI/**`
- Push to `main` that touch `src/UI/**`
- Manual run (`workflow_dispatch`)

CI executes:

- `npm ci`
- `npm run lint`
- `npm run typecheck`
- `npm run build`

### CD (deploy to your server)

Deploy job runs automatically after CI when pushing to `main`.

Set these GitHub Actions secrets before first deploy:

- `UI_DEPLOY_HOST`: server host or IP
- `UI_DEPLOY_PORT`: SSH port (optional, defaults to `22`)
- `UI_DEPLOY_USER`: SSH user
- `UI_DEPLOY_SSH_KEY`: private key content used by Actions
- `UI_DEPLOY_PATH`: target folder to host static files
- `UI_DEPLOY_POST_CMD`: optional post-deploy command on server

Deployment behavior:

1. Download built `dist` artifact
2. Connect over SSH
3. Ensure remote target folder exists
4. Sync `dist` to server with `rsync --delete`
5. Run optional post-deploy command (for example reload nginx)

Example `UI_DEPLOY_POST_CMD` values:

```bash
sudo systemctl reload nginx
```

or

```bash
pm2 reload ui
```
