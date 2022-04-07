CERTCLILib:
Due to our build servers being Win7 and majority of dev systems being Win10 there is a descripancy in the naming of some COM libraries.
This library is used by MeshServersCommon project.

Error:
error CS0246: The type or namespace name 'CERTCLILib' could not be found (are you missing a using directive or an assembly reference?)
See: 
https://stackoverflow.com/questions/23445996/com-reference-name-changes-across-versions-win7-certclientlib-to-win8-certclili



CERTADMLib:
On Windows server 2019, go to C:\Windows\System32 folder, and run the following command. "tlbimp.exe" is from Visual Studio developer command promt.
tlbimp certadm.dll /machine: x64 /out: Interop.CERTADMLib.dll /product: "Assemby imported from type library certadm.dll"

CERTADM is only available in Windows server SKU. If your application uses Interop.CERTADMLib.dl on Windows 10, then it will throw the following exception.
That is fine since EMA server does not support Win client (win 10).  802.1X testing are done in the kit which uses Windows server also.

The exception thrown on Windows 10:
System.Runtime.InteropServices.COMException: 
'Retrieving the COM class factory for component with CLSID {37EABAF0-7FB6-11D0-8817-00A0C903B83C} failed due to the following error: 80040154 Class not registered (Exception from HRESULT: 0x80040154 (REGDB_E_CLASSNOTREG)).'
