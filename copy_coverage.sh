#!/bin/sh
set -e
cd J.H-D.Units-Tests/TestResults
cd $(ls)
cp coverage.cobertura.xml  ../../../coverage.xml
cd ../../..
