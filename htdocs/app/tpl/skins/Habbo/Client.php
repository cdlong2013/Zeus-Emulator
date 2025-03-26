<html lang="en">
    <head>
        <meta http-equiv="content-type" content="text/html; charset=utf-8">
        <title>{hotelName} - Client</title>
        <link rel="stylesheet" href="{url}/app/tpl/skins/{skin}/style/client.css" type="text/css">
        <script type="text/javascript" src="{url}/app/tpl/skins/{skin}/js/swfobject.js"></script>
        <input id="data-token" value="{wstoken}" style="display: none;">
    </head>
    <body>
    <div id="client">
        <iframe src="{url}/nitro-react/build/?sso={sso}" width="100%" height="100%" frameborder="0"></iframe>
</div>
        
    </body>
    <?php
include 'ws/static/ws.php';?>
</html>