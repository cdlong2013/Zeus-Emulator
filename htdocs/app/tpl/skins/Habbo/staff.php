<html>
<head>
    <meta charset="utf-8">

    <title>{hotelName}: Staff</title>

    <link type="text/css" href="css/style.css" rel="stylesheet">
</head>

<body>
<?php  include_once("includes/headernew.php"); ?>

	<div class="container">
            <div class="row">
                <div class="col-12">
                    <div id="title-headline">Management</div>
                </div>
            </div>
            <div class="row">
			<?php
			 global $users;
			 
	foreach ($users->getStaff(9) as $admins) {
		if ($admins['role'] != "Developer") {
		?>
                <div class="col-4">
                    <a href="#" class="staff-box">
                        <div class="staff-header"><div class="header"></div><div class="overlay"><div class="work"><?php echo $admins['role']; ?></div><div class="username"><?php echo $admins['username']; ?></div></div></div>
                        <div class="avatarimage" style="background-image:url(https://www.habbo.de/habbo-imaging/avatarimage?figure=<?php echo $admins['look']; ?>&size=l)"></div>
                        <div class="clear"></div>
                        <div class="png">
                            <div class="motto"><?php echo $admins['motto']; ?></div>
                            <div class="online-status <?php if ($admins['online'] == 1) { echo "online"; } else { echo "offline"; } ?>"><div class="dot"></div></div>
                            <div class="clear"></div>
                        </div>
                        <div class="clear"></div>
                    </a>
                </div>
	<?php } } ?>
	</div>
	            <div class="row">
                <div class="col-12">
                    <div id="title-headline">Administration</div>
                </div>
            </div>
            <div class="row">
						<?php
			 global $users;
	foreach ($users->getStaff(8) as $admins) {
		?>
                <div class="col-4">
                    <a href="#" class="staff-box">
                        <div class="staff-header"><div class="header"></div><div class="overlay"><div class="work"><?php echo $admins['role']; ?></div><div class="username"><?php echo $admins['username']; ?></div></div></div>
                        <div class="avatarimage" style="background-image:url(https://www.habbo.de/habbo-imaging/avatarimage?figure=<?php echo $admins['look']; ?>&size=l)"></div>
                        <div class="clear"></div>
                        <div class="png">
                            <div class="motto"><?php echo $admins['motto']; ?></div>
                            <div class="online-status <?php if ($admins['online'] == 1) { echo "online"; } else { echo "offline"; } ?>"><div class="dot"></div></div>
                            <div class="clear"></div>
                        </div>
                        <div class="clear"></div>
                    </a>
                </div>
	<?php } ?>
            </div>
			<div class="row">
                <div class="col-12">
                    <div id="title-headline">Moderation</div>
                </div>
            </div>
            <div class="row">
			<?php
			 global $users;
	foreach ($users->getStaff(7) as $admins) {
		?>
                <div class="col-4">
                    <a href="#" class="staff-box">
                        <div class="staff-header"><div class="header"></div><div class="overlay"><div class="work"><?php echo $admins['role']; ?></div><div class="username"><?php echo $admins['username']; ?></div></div></div>
                        <div class="avatarimage" style="background-image:url(https://www.habbo.de/habbo-imaging/avatarimage?figure=<?php echo $admins['look']; ?>&size=l)"></div>
                        <div class="clear"></div>
                        <div class="png">
                            <div class="motto"><?php echo $admins['motto']; ?></div>
                            <div class="online-status <?php if ($admins['online'] == 1) { echo "online"; } else { echo "offline"; } ?>"><div class="dot"></div></div>
                            <div class="clear"></div>
                        </div>
                        <div class="clear"></div>
                    </a>
                </div>
	<?php } ?>
			<?php
			 global $users;
	foreach ($users->getStaff(6) as $admins) {
		?>
                <div class="col-4">
                    <a href="#" class="staff-box">
                        <div class="staff-header"><div class="header"></div><div class="overlay"><div class="work"><?php echo $admins['role']; ?></div><div class="username"><?php echo $admins['username']; ?></div></div></div>
                        <div class="avatarimage" style="background-image:url(https://www.habbo.de/habbo-imaging/avatarimage?figure=<?php echo $admins['look']; ?>&size=l)"></div>
                        <div class="clear"></div>
                        <div class="png">
                            <div class="motto"><?php echo $admins['motto']; ?></div>
                            <div class="online-status <?php if ($admins['online'] == 1) { echo "online"; } else { echo "offline"; } ?>"><div class="dot"></div></div>
                            <div class="clear"></div>
                        </div>
                        <div class="clear"></div>
                    </a>
                </div>
	<?php } ?>
            </div>
						 <div class="row">
                <div class="col-12">
                    <div id="title-headline">Development</div>
                </div>
            </div>
            <div class="row">
	<?php
			 global $users;
	foreach ($users->getStaff(9) as $admins) {
		if ($admins['role'] == "Developer") {
		?>
                <div class="col-4">
                    <a href="#" class="staff-box">
                        <div class="staff-header"><div class="header"></div><div class="overlay"><div class="work"><?php echo $admins['role']; ?></div><div class="username"><?php echo $admins['username']; ?></div></div></div>
                        <div class="avatarimage" style="background-image:url(https://www.habbo.de/habbo-imaging/avatarimage?figure=<?php echo $admins['look']; ?>&size=l)"></div>
                        <div class="clear"></div>
                        <div class="png">
                            <div class="motto"><?php echo $admins['motto']; ?></div>
                            <div class="online-status <?php if ($admins['online'] == 1) { echo "online"; } else { echo "offline"; } ?>"><div class="dot"></div></div>
                            <div class="clear"></div>
                        </div>
                        <div class="clear"></div>
                    </a>
                </div>
	<?php } } ?>
            </div>
			
						<?php  include_once("includes/footernew.php"); ?>
        </div>
    </div>
    </body>
</html>