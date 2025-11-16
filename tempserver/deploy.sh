#!/bin/bash

if [ "$(id -u)" -ne 0 ]; then
        echo 'This script must be run by root' >&2
        exit 1
fi

apt install unzip

curl -fsSL https://fnm.vercel.app/install | bash

source /root/.bashrc

fnm use --install-if-missing 22

npm install -g pm2

wget "https://raw.githubusercontent.com/4UPanElektryk/SimpleWebDash/refs/heads/master/tempserver/temp.js"

pm2 start ./temp.js

pm2 save

pm2 startup
