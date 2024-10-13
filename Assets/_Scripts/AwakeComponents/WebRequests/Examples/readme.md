## Face Swap request


#### How to install:

```
1. Install newtonsoft-json package

Add to Packages/manifest.json
"com.unity.nuget.newtonsoft-json": "3.0.1"
```

```
2. Install Docker

Link to download: https://www.docker.com/products/docker-desktop

```

```
3. Ectract FaceFusionApi to the root on system disk

Link to download: https://drive.google.com/file/d/11HOYB3gmL8BHRQaZrZEgV273TOq4DoX5/view?usp=drive_link
```

```
4. Run FaceFusionApi use Docker

4.1 open project folder
4.2 run terminal in folder
4.3 run command docker-compose up --build -d

```

```

#### How to use:

```

```
1. Add httpRequest to the scene
```

```
2. Use method httpRequest.SendRequestHttpClient

Use events onResponse and onError to get response from server
```