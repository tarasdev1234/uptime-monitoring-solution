#!/bin/bash
cd "$( dirname "${BASH_SOURCE[0]}" )"
find . -name "*.tar" -exec docker load -i '{}' \;
