#!/bin/sh
BASEDIR=$(dirname "$0")
cd $BASEDIR/../
dotnet user-secrets init

while getopts c:p:a: flag
do
    case "${flag}" in
        c) certificatefilename=${OPTARG};;
        p) certificatePassword=${OPTARG};;
        a) certificateAlgorithm=${OPTARG};;
    esac
done

dotnet user-secrets set "JWT:Security:Certificate"  $certificatefilename 
dotnet user-secrets set "JWT:Security:CertificatePassword" $certificatePassword
dotnet user-secrets set "JWT:Security:CertificateEncryptionAlgorithm" $certificateAlgorithm