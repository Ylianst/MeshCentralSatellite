# MeshCentral Satellite

For more information, [visit MeshCentral.com](https://www.meshcentral.com).

MeshCentral Satellite is a Windows application that can run as a normal application or as a background Windows service. Once setup to connect to MeshCentral it will automatically create 802.1x profiles in the domain controller for Intel AMT devices and can use a certificate authority in your domain to issue 802.1x certificates to Intel AMT.

## MeshCentral Configuration

This is an example of setting up 802.1x in Intel AMT without MeshCentral Satellite being involved. The MSCHAPv2 username and password is provided in the config.json of MeshCentral.

```
{
  "Settings": {

  },
  "Domains": {
    "": {
      "AmtManager": {
        "802.1x": {
          "AuthenticationProtocol": "PEAPv0/EAP-MSCHAPv2",
          "Username": "authUsername",
          "Password": "authUserPassword"
        },
        "WifiProfiles": [
          {
            "SSID": "AP-SSID-1",
            "Authentication": "wpa2-802.1x",
            "Encryption": "ccmp-aes"
          }
        ]
      }
    }
  }
}
```

The problem with this example is that all Intel AMT devices will be configured with the same 802.1x username and password this is not good for security. You can't revoke individual accounts or monitor what device is connecting since they all use the same account.

Once MeshCentral Satellite is setup, you can have a config.json that looks like this:

```
{
  "Settings": {

  },
  "Domains": {
    "": {
      "AmtManager": {
        "802.1x": {
          "AuthenticationProtocol": "EAP-TLS",
          "SatelliteCredentials": "satelliteUser",
          "AvailableInS0": true
        },
        "WifiProfiles": [
          {
            "SSID": "AP-SSID-1",
            "Authentication": "wpa2-802.1x",
            "Encryption": "ccmp-aes"
          },
          {
            "SSID": "AP-SSID-2",
            "Authentication": "wpa2-802.1x",
            "Encryption": "ccmp-aes"
          },
          {
            "SSID": "AP-SSID-3",
            "Authentication": "wpa2-psk",
            "Encryption": "ccmp-aes",
            "Password": "my-wifi-password"
          }
        ]
      }
    }
  }
}
```

In the example above, MeshCentral will configure 802.1x for the wired interface and for 2 of the 3 WIFI profiles. AP-SSID-1 and AP-SSID-2 are set to authenticate using 802.1x and AP-SSID-3 is setup with regular WPA2 password authentication.

What makes this 802.1x configuration interesting is the line "SatelliteCredentials". This indicates a MeshCentral Satellite will be connected with the user account name "satelliteUser" and to query it to setup a 802.1x profile in the Windows domain controller and issue a 802.1x authentication certificate to Intel AMT.

Another example is this:

```
{
  "Settings": {

  },
  "Domains": {
    "": {
      "AmtManager": {
        "802.1x": {
          "AuthenticationProtocol": "PEAPv0/EAP-MSCHAPv2",
          "SatelliteCredentials": "satelliteUser"
        },
        "WifiProfiles": [
          {
            "SSID": "AP-SSID-1",
            "Authentication": "wpa2-802.1x",
            "Encryption": "ccmp-aes"
          }
        ]
      }
    }
  }
}
```

In this example, the Intel AMT wired interface is configured with 802.1x along with a single WIFI profile. This time, instead of EAP-TLS being used for authentication, PEAPv0/EAP-MSCHAPv2 will be used. MeshCentral Satellite will be queried, but this time, a 802.1x account will be created in the domain with a username and random password. The password will be sent back to MeshCentral and set into Intel AMT.

## MeshCentral Satellite Setup

You need to run MeshCentral Satellite on a computer that is joined to your domain and run it with sufficient rights that it can create LDAP computer objects and have access to the domain Certificate Authority so it can request that certificates be signed.

You will probably want to run MeshCentral Satellite as a normal Windows application at first to make sure everything works before running it as a background Windows service. You can start by going in the "Settings" option in the menus and setting up the MeshCentral server's host name and login username and password. You also need to setup that certificate authority to use and certificate template. If a certificate authority is not setup, only PEAPv0/EAP-MSCHAPv2 will be supported.

You can also indicate what domain security groups a computer must be joined to when a new 802.1x computer is created.

Once done, you can login to MeshCentral and the MeshCentral Satellite is ready to receive requests. You can use the "Testing" menu to create and remove a test computer from the domain. This is useful to make sure everything is working well before getting requests from MeshCentral.

## Video Tutorials
You can watch many tutorial videos on the [MeshCentral YouTube Channel](https://www.youtube.com/channel/UCJWz607A8EVlkilzcrb-GKg/videos). There is one video on how to setup Intel AMT with 802.1x without MeshCentral Satellite, this is a good way to get started.

Basic Intel AMT 802.1x with JumpCloud.  
[![MeshCentral - Basic Intel AMT 802.1x with JumpCloud](https://img.youtube.com/vi/tKI9UJ1O15M/mqdefault.jpg)](https://www.youtube.com/watch?v=tKI9UJ1O15M)

MeshCentral Satellite & Advanced Intel AMT 802.1x.  
[![MeshCentral - Satellite & Advanced Intel AMT 802.1x](https://img.youtube.com/vi/1otWwjtFBIA/mqdefault.jpg)](https://www.youtube.com/watch?v=1otWwjtFBIA)

## Feedback
If you encounter a problem or have a suggestion to improve the product, you may file an [issue report](https://github.com/Ylianst/MeshCentral/issues/)

If you are filing a problem report, you should include:
* The version of the software you are using
* The Operating System and version
* The observed output
* The expected output
* Any troubleshooting you took to resolve the issue yourself
* Any other similar reports

If you are having issues with the following other products, you should file a report on their respective issue pages  
[MeshCentralSatellite](https://github.com/Ylianst/MeshCentralSatellite/issues)
[MeshCentral](https://github.com/Ylianst/MeshCentral/issues)

## License
This software is licensed under [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0)
