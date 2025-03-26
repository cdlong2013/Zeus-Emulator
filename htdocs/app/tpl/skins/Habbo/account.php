<html>
<head>
    <meta charset="utf-8">

    <title>{hotelName}: Account</title>

    <link type="text/css" href="{url}/css/style.css" rel="stylesheet">
	<link type="text/css" href="{url}/css/settings.css" rel="stylesheet">
</head>

<body>
<?php  include_once("includes/headernew.php"); ?>
	<div class="container">
		<div class="row">

<div class="col-3">
    <a href="{url}/account" id="settings-navigation-box" class="selected">
        <div class="png20">
            <i class="far fa-lock-open-alt icon"></i>
            <div class="settings-title">Account Settings</div>
            <div class="settings-desc">Change Password</div>
        </div>
        <div class="clear"></div>
    </a>
</div>
<div class="col-9">
<?php if(isset($template->form->error)) { ?>
    <div class="alert failed"><?php echo $template->form->error; ?></div>
<?php } ?>
    <div id="content-box" style="height:570px">
        <div class="title-box png20">
            <div class="title">Account Password</div>
        </div>

        <div class="png20">
            <form method="post">
                <label for="old-password">Old Password</label>
                <input type="password" id="old-password" name="acc_old_password">
                <div class="desc" style="margin: 0 0 20px 0">Your current password is the password you use to login to the main website.</div>
                <div class="line"></div>

                <label for="new-password">New Password</label>
                <input type="password" id="new-password" name="acc_new_password">
                <div class="desc">Enter your new password.</div>
				
				<label for="new-password-again">Repeat New Password</label>
                <input type="password" id="new-password-again" name="acc_new_password_again">
                <div class="desc">Enter your new password again to ensure its correct.</div>

                <button type="submit" name="account" class="btn green save">Save changes</button>
            </form>
        </div>
    </div>
</div>

			<?php  include_once("includes/footernew.php"); ?>
        </div>
    </div>
    </body>
</html>