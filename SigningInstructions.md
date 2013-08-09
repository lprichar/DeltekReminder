Signing Assemblies
======================

The origianl error is 'Cannot import the following keyfile: blah.pfx. The keyfile may be password protected'.  From:
http://stackoverflow.com/questions/2815366/cannot-import-the-following-keyfile-blah-pfx-the-keyfile-may-be-password-prote

1. Create a new pfx certificate with openssl

        "C:\Program Files (x86)\GnuWin32\bin\openssl.exe" pkcs12 -in CertumOpenSourceCert.pfx -out backupcertfile.key
        "C:\Program Files (x86)\GnuWin32\bin\openssl.exe" pkcs12 -export -out CertumOpenSourceCertToSignWith.pfx -keysig -in backupcertfile.key

2. Reference new pfx, try to compile

  You should get the dreaded "error MSB3325: Cannot import the following key file: myCert.pfx. The key file may be password protected. To correct this, try to import the certificate again or manually install the certificate to the Strong Name CSP with the following key container name: VS_KEY_B763CB2413AC1708"

3. Import the new SSL Certificate with sn

        sn -i CertumOpenSourceCertToSignWith.pfx VS_KEY_83C9EB0853B733C5

Resigning Referenced Assemblies
======================

1. Disassemble

    cd C:\dev\DeltekReminder\packages\Hardcodet.Wpf.TaskbarNotification.1.0.4.0\lib\net40

    ildasm /all /out=Hardcodet.Wpf.TaskbarNotification.il Hardcodet.Wpf.TaskbarNotification.dll

2. Delay Sign    

    ilasm /dll /key="C:\Users\Lee\Dropbox\Personal\CertiumOpenSourceCertToSignWith-PublicKey.snk" Hardcodet.Wpf.TaskbarNotification.il > out.txt

3. Final Sign

    sn -R Hardcodet.Wpf.TaskbarNotification.dll C:\Users\Lee\Dropbox\Personal\CertumOpenSourceCertToSignWith.pfx


References
------------------------


Disassembly stuff from: http://buffered.io/posts/net-fu-signing-an-unsigned-assembly-without-delay-signing/

Re-signing stuff from: http://ianpicknell.blogspot.com/2009/12/adding-strong-name-to-third-party.html