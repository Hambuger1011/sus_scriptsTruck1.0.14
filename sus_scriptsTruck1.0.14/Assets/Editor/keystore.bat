::生成签名
::keytool -exportcert -alias 别名-keystore  证书路径 -storepass 证书查看密码-file nuoyuan.cer
keytool -genkey -keyalg RSA -validity 36500 -storepass "onyxgame" -keypass "onyxgame" -keystore "CH_SPAIN.keystore" -alias "CH_SPAIN"