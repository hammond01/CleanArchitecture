#!/bin/bash

echo "================================="
echo "Order Management Database Migrator"
echo "================================="
echo

if [ -z "$1" ]; then
    echo "Running migration with default settings..."
    dotnet run migrate
else
    echo "Running command: $1"
    dotnet run "$1"
fi

echo
echo "Migration completed."
