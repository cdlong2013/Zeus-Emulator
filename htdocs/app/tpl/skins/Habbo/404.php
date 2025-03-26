<html>
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">

        <title>{hotelname}</title>

        <link type="text/css" href="css/index.css" rel="stylesheet">
    </head>

    <body>
	<?php if(isset($template->form->error)) { ?>
        <div class="error-msg"><?php echo $template->form->error; ?></div>
	<?php } ?>
		
        <div class="hero">
            <div class="hotel"></div>
        </div>

        <div id="header-content">
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <div class="logo"></div>
                        <div class="online-count"><b>{online}</b> players online</div>
                    </div>
                </div>
            </div>
        </div>
        <div class="container">
            <div class="row">
                <div class="col-md-5">
                    <div class="login-position">
                        <h1>404</h1>
            </div>
        </div>
    </body>
</html>