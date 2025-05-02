#  TokenManager

> Api'den veri çekmeden önce access token alınması gerekiyor.  
> Token'ın süresi var, gereksiz yere tekrar alınmaası lazım.  
> Bu yapı tokenı bir kere alıp süresi geçene kadar kullanır.  
> Süresi geçince tekrar alır. 

##  Nasıl Çalışır

- Uygulama çalışırken token boşsa veya süresi geçmişe yeni token alınır.
- Token bellekte tutulur, geçerli süre boyunca aynı token kullanılır.
- Her token isteğiyle beraber access_token, expires_in ve token_type gelir
- expires_in saniye cinsinden geldiği için token'ın geçerlilik süresi hesaplanır.
