#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
gcc -shared -fPIC -o DeliveryApp/libdelivery_native.so native/delivery_native.c
echo "Built DeliveryApp/libdelivery_native.so"
