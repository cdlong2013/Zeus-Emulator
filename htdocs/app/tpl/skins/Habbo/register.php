<html>
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">

        <title>{hotelname}</title>

        <link type="text/css" href="css/register.css" rel="stylesheet">
		<style>
.gender-selector input{
    margin:0;padding:0;
    -webkit-appearance:none;
       -moz-appearance:none;
            appearance:none;
}

.male{background-image:url({url}/img/male.png);}
.female{background-image:url({url}/img/female.png);}

.gender-selector input:active +.gender{opacity: .9;}
.gender-selector input:checked +.gender{
    -webkit-filter: none;
       -moz-filter: none;
            filter: none;
}
.gender{
    cursor:pointer;
    background-repeat:no-repeat;
    display:inline-block;
    width:50px;
    height:50px;
    -webkit-transition: all 100ms ease-in;
       -moz-transition: all 100ms ease-in;
            transition: all 100ms ease-in;
    -webkit-filter: brightness(1.8) grayscale(1) opacity(.7);
       -moz-filter: brightness(1.8) grayscale(1) opacity(.7);
            filter: brightness(1.8) grayscale(1) opacity(.7);
}
.gender:hover{
    -webkit-filter: brightness(1.2) grayscale(.5) opacity(.9);
       -moz-filter: brightness(1.2) grayscale(.5) opacity(.9);
            filter: brightness(1.2) grayscale(.5) opacity(.9);
}
</style>
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
                        <h2>Register</h2>
                        <form method="post" id="form" name="regformitem">
                            <label class="lbl" for="reg_username">Username</label>
                            <input type="text" name="reg_username" id="user">
							<label class="lbl" for="reg_gender">Gender</label>
							<div class="gender-selector">
								<input checked="checked" id="male" type="radio" name="reg_gender" value="M" />
								<label class="gender male" for="male"></label>
								<input id="female" type="radio" name="reg_gender" value="F" />
								<label class="gender female" for="female"></label>
							</div>

                            <label class="lbl" for="reg_password">Password</label>
                            <input type="password" name="reg_password" id="password">
							
                            <label class="lbl" for="reg_rep_password">Confirm Password</label>
                            <input type="password" name="reg_rep_password" id="password">
                            <button type="submit" name="register" class="btn big green login-button">Register</button>
                        </form>
						</div>
						</div>
            </div>
        </div>
    </body>
</html>