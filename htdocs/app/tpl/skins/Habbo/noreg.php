
<!DOCTYPE html>
<!--[if lt IE 7]>      <html class="no-js lt-ie9 lt-ie8 lt-ie7"> <![endif]-->
<!--[if IE 7]>         <html class="no-js lt-ie9 lt-ie8"> <![endif]-->
<!--[if IE 8]>         <html class="no-js lt-ie9"> <![endif]-->
<!--[if gt IE 8]><!--> <html class="no-js"> <!--<![endif]-->
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <title>{hotelname} Hotel - Make friends, join the fun, get noticed! </title>
    <meta name="viewport" content="width=device-width">

    <script>
        var andSoItBegins = (new Date()).getTime();
        var habboPageInitQueue = [];
        var habboStaticFilePath = "https://habboo-a.akamaihd.net/habboweb/63_1dc60c6d6ea6e089c6893ab4e0541ee0/1633/web-gallery";
    </script>
    <link rel="shortcut icon" href="https://habboo-a.akamaihd.net/habboweb/63_1dc60c6d6ea6e089c6893ab4e0541ee0/1633/web-gallery/v2/favicon.ico" type="image/vnd.microsoft.icon" />

    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Ubuntu:400,700,400italic,700italic">

<link rel="stylesheet" href="{url}/app/tpl/skin/{skin}/styles/frontpage_new.css" type="text/css" />
<script src="{url}/app/tpl/skin/{skin}/js/v3_landing_top.js" type="text/javascript"></script>

        <meta name="description" content="Check into the world's largest virtual hotel for FREE! Meet and make friends, play games, chat with others, create your avatar, design rooms and more..." />
        <meta name="keywords" content="habbo hotel, virtual, world, social network, free, community, avatar, chat, online, teen, roleplaying, join, social, groups, forums, safe, play, games, online, friends, teens, rares, rare furni, collecting, create, collect, connect, furni, furniture, pets, room design, sharing, expression, badges, hangout, music, celebrity, celebrity visits, celebrities, mmo, mmorpg, massively multiplayer" />

<script src="//cdn.optimizely.com/js/13389159.js"></script>
    <meta name="build" content="63-BUILD2030 - 22.03.2013 11:10 - com" />
    <meta name="csrf-token" content="89fe674d03"/>
</head>
<body>

<div id="overlay"></div>


<div id="change-password-form" class="overlay-dialog" style="display: none;">
    <div id="change-password-form-container" class="clearfix form-container">
        <h2 id="change-password-form-title" class="bottom-border">Forgot Password?</h2>
        <div id="change-password-form-content" style="display: none;">
            <form method="post" action="{url}/account/password/identityResetForm" id="forgotten-pw-form">
                <input type="hidden" name="page" value="/?changePwd=true" />
                <span>Type in your {hotelname} account email address:</span>
                <div id="email" class="center bottom-border">
                    <input type="text" id="change-password-email-address" name="emailAddress" value="" class="email-address" maxlength="48"/>
                    <div id="change-password-error-container" class="error" style="display: none;">Please enter a correct email address</div>
                </div>
            </form>
            <div class="change-password-buttons">
                <a href="#" id="change-password-cancel-link">Cancel</a>
                <a href="#" id="change-password-submit-button" class="new-button"><b>Send Email</b><i></i></a>
            </div>
        </div>
        <div id="change-password-email-sent-notice" style="display: none;">
            <div class="bottom-border">
                <span>Hey, we just sent you an email with a link that lets you reset your password.<br>
<br>

NOTE! Remember to check your "junk" folder too!</span>
                <div id="email-sent-container"></div>
            </div>
            <div class="change-password-buttons">
                <a href="#" id="change-password-change-link">Back</a>
                <a href="#" id="change-password-success-button" class="new-button"><b>OK</b><i></i></a>
            </div>
        </div>
    </div>
    <div id="change-password-form-container-bottom" class="form-container-bottom">
</div>
</div>


<header>
    <div id="border-left"></div>
    <div id="border-right">
</div>

<?php if(isset($template->form->error)) { ?>
                    <div id="loginerrorfieldwrapper">
                        <div id="loginerrorfield">
                            <div><?php echo $template->form->error; ?></div>
                        </div>
                    </div>
                <?php } ?>


<div id="login-form-container">
    <a href="#home" id="habbo-logo"></a>

    
</div>

    <div id="alerts">
<noscript>
<div id="alert-javascript-container">
    <div id="alert-javascript-title">
        Missing JavaScript support
    </div>
    <div id="alert-javascript-text">
        Javascript is disabled on your browser. Please enable JavaScript or upgrade to a Javascript-capable browser to use {hotelname} :)
    </div>
</div>
</noscript>

<div id="alert-cookies-container" style="display:none">
    <div id="alert-cookies-title">
        Missing cookie support
    </div>
    <div id="alert-cookies-text">
        Cookies are disabled on your browser. Please enable cookies to use {hotelname}.
    </div>
</div>
    </div>
    <div id="top-bar-triangle"></div>
    <div id="top-bar-triangle-border"></div>
</header>

        <li id="registration-anchor">

<div id="registration-form">
    <div id="registration-form-header">
        <h2>User ID</h2>
        <p>Fill in these details to begin:</p>
    </div>
    <div id="page-content-padding">

<?php if(isset($template->form->error)) {  echo '<div style="text-align:center; color:red; font-weight: bold;">'.$template->form->error.'</div>'; }?>
<br />
<form action="register" method="post" id="registerForm">
<div id="registration-form-main">
<div id="registration-form-main-left">
<label for="email-address">Username</label>
<label for="email-address" class="details">You'll need to use this <b>to log in</b> to {hotelname} in the future. Please use a valid address.</label>
<input type="text" autocomplete="on" name="reg_username" value="<?php echo $_POST['reg_username'] ?>" id="username" placeholder="Username" class="box1"><br /><br />
<label for="email-address">Email</label>
<label for="email-address" class="details">We will use this to restore your account if you ever lose access. Your Email will never be shared publicly.</label>
<input type="text" autocomplete="on" name="reg_email" id="email" value="<?php echo $_POST['reg_email']; ?>" placeholder="Email Address" class="box1"><br /><br />
</div>


<div id="registration-form-main-right">
<label for="register-password">Password</label>
<label for="register-password" class="details">Password must be at least <b>6 characters </b>long and include <b>letters and numbers</b></label>
<input type="password" autocomplete="off" name="reg_password" id="password" placeholder="Password" class="box1"><br /><br />
<label for="password2"><b>Repeat Password</b></label> 
<label for="register-password" class="details">Please vertify<b>Your password</b>to make sure you didn't make a mistake.</b></label>
<input type="password" autocomplete="off" name="reg_rep_password" id="password1" placeholder="Confirm Password" class="box1"><br /><br />

<input type="hidden" name="reg_seckey" value="1234" />
<input type="hidden" name="register" value="register" />
<input class="regbtn" type="submit" name="register" onClick="checkForm(); return false;" value="Register" />
</div>
</div>
</form>
<br /><br /><br />

								</div>
							</div>
								
								
    
</div>

        </li>
    </ul>
</div>

<footer>

    <div id="footer-content">
        <div id="footer"><a href="http://help.habbo.com">Customer Support</a> / <a href="https://help.habbo.com/forums/144065-information-for-parents">Parents</a> / <a href="https://help.habbo.com/entries/23096348-Terms-of-Service-and-Privacy-Policy" target="_new">Terms of Use & Privacy Policy</a> / <a href="https://help.habbo.com/entries/278050-infringements-policy" target="_new">Infringements</a> / <a href="http://www.habbo.com/safety/habbo_way" target="_new">{hotelname} Way</a> / <a href="http://www.habbo.com/safety/safety_tips">Safety</a> / <a href="http://issuu.com/sulake/docs/habbo_media_pack_2013_v3.0_com?mode=window&viewMode=doublePage" target="_blank">For Advertisers</a></div>
        <div id="copyright">© 2004 - 2013 Sulake Corporation Oy. HABBO is a registered trademark of Sulake Corporation Oy in the European Union, the USA, Japan, the People's Republic of China and various other jurisdictions. All rights reserved.</div>
    </div>
    <div id="sulake-logo"><a href="http://www.sulake.com"></a></div>
</footer>


<script src="{url}/app/tpl/skin/{skin}/js/v3_landing_bottom.js" type="text/javascript"></script>
<!--[if IE]><script src="https://habboo-a.akamaihd.net/habboweb/63_1dc60c6d6ea6e089c6893ab4e0541ee0/1633/web-gallery/static/js/v3_ie_fixes.js" type="text/javascript"></script>
<![endif]-->




</body>
</html>