#!/usr/bin/env bash
set -euo pipefail

### ==== CONFIG ==== ###
APP_NAME="nhts"
FULL_APP_NAME="Node Health & Telemetry Server"
GITHUB_REPO="4UPanElektryk/SimpleWebDash"   # ← e.g. janek/myapp
ARCH="amd64"              # change to arm64 if needed
APP_USER="nhts"
APP_DIR="/opt/${APP_NAME}"
BIN_PATH="${APP_DIR}/${APP_NAME}"
SERVICE_FILE="/etc/systemd/system/${APP_NAME}.service"
### ================== ###

echo "[ INFO ] Installing ${APP_NAME} from GitHub releases"

# --- sanity checks ---
if [[ $EUID -ne 0 ]]; then
  echo "[ FATAL ] Please run as root"
  exit 1
fi

if ! command -v curl >/dev/null; then
  echo "[ FATAL ] curl is required"
  exit 1
fi

# --- detect latest release ---
echo "[ INFO ] Fetching latest release INFO"
LATEST_URL=$(curl -fsSL \
  "https://api.github.com/repos/${GITHUB_REPO}/releases/latest" \
  | grep browser_download_url \
  | grep "linux-${ARCH}" \
  | cut -d '"' -f 4)

if [[ -z "${LATEST_URL}" ]]; then
  echo "[ FATAL ] Could not find linux-${ARCH} binary in latest release"
  exit 1
fi

echo "[ INFO ] Latest binary: ${LATEST_URL}"

# --- create user ---
if ! id "${APP_USER}" &>/dev/null; then
  echo "[ INFO ] Creating system user ${APP_USER}"
  useradd \
    --system \
    --no-create-home \
    --shell /usr/sbin/nologin \
    "${APP_USER}"
fi

# --- install binary ---
echo "[ INFO ] Installing binary"
mkdir -p "${APP_DIR}"

curl -fsSL "${LATEST_URL}" -o "${BIN_PATH}"
chmod 755 "${BIN_PATH}"
chown -R "${APP_USER}:${APP_USER}" "${APP_DIR}"

# --- create systemd service ---
if [[ ! -f "${SERVICE_FILE}" ]]; then
  echo "[ INFO ] Writing systemd service"

  cat > "${SERVICE_FILE}" <<EOF
[Unit]
Description=${FULL_APP_NAME} service
After=network.target pve-cluster.service
Wants=network.target

[Service]
Type=simple
User=${APP_USER}
Group=${APP_USER}
WorkingDirectory=${APP_DIR}
ExecStart=${BIN_PATH}

Restart=on-failure
RestartSec=5s

# Optional env file
# EnvironmentFile=/etc/${APP_NAME}.env

StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
EOF
fi

# --- enable & start ---
echo "[ INFO ] Reloading systemd"
systemctl daemon-reexec
systemctl daemon-reload

echo "[ INFO ] Enabling ${APP_NAME}"
systemctl enable "${APP_NAME}"

echo "[ INFO ] Restarting ${APP_NAME}"
systemctl restart "${APP_NAME}"

echo "[ SUCCESS ] ${APP_NAME} installed / updated successfully"
systemctl --no-pager status "${APP_NAME}"
