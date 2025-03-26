window.onload = function() {

	var RP = '127.0.0.1';
	var RPurl = 'http://127.0.0.1';
	var Port = '3000';
	var host = "ws://"+ RP +":"+ Port +"/";
    var ws = new WebSocket(host);
	var username = document.getElementById("myname").innerHTML;
    var look = 'http://www.habbo.com/habbo-imaging/avatarimage?figure=';       
    var bleeding = false;
    var lock = false;
    var isbot = false;
    var wl = false;
    var info_message = false;
	var vs1 = document.getElementById('vs1');
	var vs2 = document.getElementById('vs2');
    var notify = document.getElementById('notify');
    var spin1 = document.getElementById('slotspin1');
    var spin2 = document.getElementById('slotspin2');
    var spin3 = document.getElementById('slotspin3');
    var spin4 = document.getElementById('spin4');
    var spin5 = document.getElementById('spin5');
    var spin6 = document.getElementById('spin6');
	var copbeep = document.getElementById('copbeep');
    var openw = 0;
    var marcos = false;
    var gangwindow = false;
    var taxiwindow = false;
    var working = false;
    var color;
    var opencfh = false;
    var opengang = false;
    var istimer = false;
    var curtime = 0;
    var cursec = 0;
    var timertype = 'null';
    var trash = false;
    var settings = 0;
    var atm;
    var stat_arrow = true;
    var targetstat_arrow = false;
    var color1;
    var color2;
    var copblink = 0;
	var policecall = 0;
	var copbeeping = false;
	var ItemDrag = false;
	var CurItem = false;
	var prevItem = '';
	var ItemMove = '';
	var ItemType = '';
	var soulmate = '';
	var checkbox1 = 0;
	var checkbox2 = 0;
	var checkbox3 = 0;
	var token = $("#data-token").val();

    function connect() {
        ws.onopen = function() {
            console.log('connection open!');
        }
        ws.onmessage = function(evt) {
            if (evt.data == 'searching') {
                ws.send('Eventsearch.Name' + username + '.Data.ExtraData.Token' + token);
                return;
            }
            console.log('data received!')
            
            user = JSON.parse(evt.data);
                           
           
            if (user == undefined || user == null || user == "")
                return;
            var command = user.name;
            switch (command) {
				case 'item_circle':
				if (user.info == 1){
				    $('#circle_item1').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/items/' + user.item + '.png)');
					$('#wepcircle1').fadeIn();
					$('#circle_hp1').animate({width: ((user.hp / user.maxhp) * 100) + '%'}, 200);
				}
				else if (user.info == 2){
				    $('#circle_item2').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/items/' + user.item + '.png)');
					$('#wepcircle2').fadeIn();
					$('#circle_hp2').animate({width: ((user.hp / user.maxhp) * 100) + '%'}, 200);
				}
				else if (user.info == 3)
					$('#wepcircle1').fadeOut();
				else if (user.info == 4)
					$('#wepcircle2').fadeOut();
				else if (user.info == 5){
					$('#wepcircle2').hide();
					$('#circle_item1').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/items/' + user.item + '.png)');
					$('#circle_hp1').animate({width: ((user.hp / user.maxhp) * 100) + '%'}, 200);
				}
					break;
                case 'slot':
                    $('#slottitle').text('$5 to spin!');
                    $('#slotmachine').show();
                    break;
                case 'bio':
					$('#editbio').show();
			     	$('#bio').show();
			     	$('#checkbio').hide();
				    $('#cancelbio').hide();
				    $("#bio").text(user.bio);
                    $('#bio_input').val('');
                    $("#bio_input").hide();
                    break;
                case 'fullstats':
                    $('#fullstats').show();
                    $('#profile').css('content', 'url(' + look + user.look + '&direction=4&head_direction=3&headonly=0&gesture=sml)');
                    $('#online').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/' + user.online + '.png)');
					if (user.soulmate.length > 0){
						$("#statname").css({'width': 'auto'});
				    soulmate = user.soulmate;
                    document.getElementById("statname").innerHTML = '<b>' + user.username.fontcolor(user.color) + '</b> is married to';
					var position = $("#statname").position();	
                    var width = $("#statname").width();	
					if (width > 288)
						{	
                            $("#smname").css({'left': position.left + width - 280});					
						    $("#smname").css({'top': '100'});
						}						
						else {
							$("#smname").css({'left': position.left + width + 4});
							$("#smname").css({'top': '83'});
						}	
					document.getElementById("smname").innerHTML = user.soulmate;					
					}
				   else {
					   document.getElementById("statname").innerHTML = '<b>' + user.username.fontcolor(user.color) + '</b> is not in a relationship';
					   document.getElementById("smname").innerHTML = '';
					   soulmate = '';
				   }
                    $('#login').text(user.login + '');
                    $('#xp_inner').css({width: ((user.xp1 / user.xp2) * 100) + '%'}, 200);
                    $('#xp_text').text(user.xp);
                    document.getElementById("lvl").innerHTML = user.level;
                    document.getElementById("str").innerHTML = user.str;
                    document.getElementById("arrests").innerHTML = user.arrest;
                    document.getElementById("deaths").innerHTML = user.deaths;
                    document.getElementById("punches").innerHTML = user.punches;
                    document.getElementById("kills").innerHTML = user.kills;
                    document.getElementById("solowins").innerHTML = user.solowins;
                    document.getElementById("sololost").innerHTML = user.sololost;
					if (user.bio.length > 0)
					document.getElementById("bio").innerHTML = user.bio;
				    else document.getElementById("bio").innerHTML = 'I\'\m new to HabboRP!';
					 if (user.username == username) 
					 {
					$('#statstitle').text('My Profile');	
                    $('#editbio').show();
					 }					
                    else 
					{
				    $('#statstitle').text(user.username+ "'s Profile");
					$('#editbio').hide();
					}
					$("#statname").css({'width': '288px'});	
                    $("#bio_input").hide();
                    $('#bio_input').val('');
					$("#checkbio").hide();
					$("#cancelbio").hide();
					$("#bio").show();
                    if (user.gender == 'f') {
                        $('#stats_icon1').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/female_icon.png)');
                        $('#profile_bg').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/female_bg.png)');
                    } else {
                        $('#stats_icon1').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/male_icon.png)');
                        $('#profile_bg').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/male_bg.png)');
                    }
                    $('#jobtitle').text(user.jobtitle);
                    if (user.jobtitle == 'Unemployed')
					{
                        $('#showjob').hide();
						$("#jobtitle").css({'left': '173px'});
						$("#jobtitle").css({'top': '145px'});
					}
                    else {
						$("#jobtitle").css({'left': '95px'});
						$("#jobtitle").css({'top': '91px'});
                        $('#showjob').show();
                        $('#jobbadge').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/wsbadge/' + user.jobbadge + ')');
                        $('#jobrank').text(user.jobrank);
                        $('#jobdate').text(user.jobdate);
						document.getElementById("task1").innerHTML = user.task1;
						document.getElementById("task2").innerHTML = user.task2;
						document.getElementById("task3").innerHTML = user.task3;
						document.getElementById("task4").innerHTML = user.task4;
                    }
                    $("#gang_title").text(user.gangname);
                    if (user.gangname == '') {
                        $("#gang_title").text('No Gang');
						$("#gang_title").css({'left': '195px'});
						$("#gang_title").css({'top': '145px'});
                        $('#showgang').hide();
                    } else {
                        $('#showgang').show();
						$("#gang_title").css({'left': '109px'});
						$("#gang_title").css({'top': '91px'});
                        $('#gang_title').text(user.gangname);
                        $('#gang_date').text(user.gangdate);
                        $('#gangrank_name').text(user.gangrank);
                        $('#gang_color1').css("background", user.color1);
                        $('#gang_color2').css("background", user.color2);
						document.getElementById("gang_arrest").innerHTML = user.gangarrest;
						document.getElementById("gang_turfs").innerHTML = user.turf;
						document.getElementById("gang_kill").innerHTML = user.gangkill;
						document.getElementById("gang_hit").innerHTML = user.ganghit;

                    }
					 $("#pet_title").text(user.petname);
                    if (user.petname == 'No Pet') {
						$("#pet_title").css({'left': '195px'});
						$("#pet_title").css({'top': '145px'});
                        $('#showpet').hide();
                    } else {
                        $('#showpet').show();
						$('#pet_level').text(user.petlevel);
						$('#petxp_text').text(user.petxp);
						$('#petxp_inner').css({width: ((user.petxp1 / user.petxp2) * 100) + '%'}, 200);
						$("#pet_title").css({'left': '135px'});
						$("#pet_title").css({'top': '84px'});
						document.getElementById("pet_kill").innerHTML = user.petkill;
						document.getElementById("pet_hit").innerHTML = user.pethit;
						document.getElementById("pet_death").innerHTML = user.petdeath;
						document.getElementById("pet_arrest").innerHTML = user.petarrest;
						new PetPosition(user.pet);

                    }
                    break;
                case 'halert':
                    $('#halert_title').text('Message from management');
                    $('#halert_text').text(user.message);
                    $('#halert_author').text('- ' + user.author);
                    $('#halert').show();
                    break;
                case 'trade':
                    $('#trade').show();
                    break;
                case 'timer':
                    timertype = user.timerevent;
                    if (istimer) {
                        curtime = user.time;
                        cursec = user.seconds;
                    } else {
                        jQuery(function($) {
                            var Minutes = user.time,
                                display = $('#time');
                            new startTimer(Minutes, display, user.seconds);
                            istimer = true;
                        });
                    }
                    $('#timer').show();
                    break;
                case 'vault':
                    if (user.info == '1') {
                        $('#panel').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/vault/panel.png)');
                        $('#panel_input').show();
                        $('#vault').show();
                    } else if (user.info == '2') {
                        $('#panel').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/vault/screen_green.png)');
                        $('#panel_input').hide();
						 if (checkbox2 == 1)
						vs1.play();
                    } else if (user.info == '3') {
                        $('#panel').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/vault/screen_red.png)');
                        $('#panel_input').hide();
						 if (checkbox2 == 1)
						vs2.play();
                    }
                    break;
                case 'wl':
                    if (user.closewl == '1' && user.bypass != 'true') {
                        $('#wl').hide();
                        wl = false;
                        break;
                    }
                    wl = true;  
                    $('#wl').show();					
		            document.getElementById("wl_name").innerHTML = user.username.fontcolor(user.color);
					document.getElementById("wl_reason1").innerHTML = user.reason1 + user.rcount1.fontcolor("red");
					if (user.reason2 != '')
					document.getElementById("wl_reason2").innerHTML = user.reason2 + user.rcount2.fontcolor("red");
				else document.getElementById("wl_reason2").innerHTML = '';
			     	if (user.reason3 != '')
					document.getElementById("wl_reason3").innerHTML = user.reason3 + user.rcount3.fontcolor("red");
				else document.getElementById("wl_reason3").innerHTML = '';
                    $('#wl_look').css('content', 'url(' + look + user.look + '&direction=2&headonly=0)');					
                    $('#wl_online').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/' + user.online + '.png)');					
                    $('#wl_time').text('Added ' + user.time);
                    $('#wl_page').text(user.pagestart + '/' + user.pageend); 
			        if (user.job == '1'){
					  $('#wl_clear').show();	
    				var offset1 = $("#wl_name").position();
                    var divwidth1 = document.getElementById("wl_name");
                    $("#wl_clear").css({'left': offset1.left + divwidth1.clientWidth + 5}); 
				  }					
				  else  $('#wl_clear').hide();
                    break;
				case 'wl_page':
				$('#wl_page').text(user.pagestart + '/' + user.pageend); 
				break;
                case '911':
				if (policecall == 1 && user.bypass != 'null')
				{
					policecall = 0;
					$('#police').hide();
					break;
				}
				policecall = 1;
				document.getElementById("callername").innerHTML = user.username.fontcolor(user.color);
                    $('#callerlook').css('content', 'url(' + look + user.look + '&direction=2&headonly=0)');					
                    $('#callerroom').text(user.roomname + ' [' + user.roomid + ']');
                    $('#callermsg').text(user.msg);
                    $('#callertime').text(user.time);
                    $('#callpage').text(user.pagestart + '/' + user.pageend);      
                    $('#police').show();					
                    break;
				case '911timer':
			      	$('#callertime').text(user.time);
				    break;
				case 'wl_timer':
			      	$('#wl_time').text('Added '+ user.time);
					break;
                case 'changename':
                    username = user.username;
                    $('#myname').text(user.username);
                    break;
                case 'medicalert':
                    if (checkbox2== 1)
                        notify.play();
                    break;
                case 'updatemoney':
                    $('#money').text(user.money);
                    break;
                case 'achievement':
                    $('#achievebadge').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/achbadge/'+user.badge_type+'/' + user.badge_name + '.gif)');
					console.log('' + RPurl + '/app/tpl/skins/Habbo/ws/achbadge/'+user.badge_type+'/' + user.badge_name + '')
                    $('#achievetext').text(user.badge_desc);
                    if (user.badge_type == 'arrests')
                        $("#achievebadge").css('top', 15);
                    else if (user.badge_type == 'kills' || user.badge_type == 'hits' || user.badge_type == 'deaths')
                        $("#achievebadge").css('top', 11);
                    $('#achievement').fadeIn();
                    break;
                case 'achievetimer':
                    $('#achievement').fadeOut("slow");
                    break;
                case 'atm':
                    $('#atm_back').hide();
                    $('#atm_balance').hide();
                    $('#atm_title2').hide();
                    $('#atm_option4').hide();
                    $('#atm_input').val('');
                    $('#atm_title').text('- WELCOME, ' + user.title + '');
                    $('#atm_title').show();
                    $('#atm_option1').show();
                    $('#atm_option2').show();
                    $('#atm_option3').show();
                    $('#atm').show();
                    break;
                case 'atm_balance':
                    $('#atm_title').hide();
                    $('#atm_option1').hide();
                    $('#atm_option2').hide();
                    $('#atm_option3').hide();
                    $('#atm_option4').hide();
                    $('#atm_balance').text('$' + user.balance + '');
                    $('#atm_back').show();
                    $('#atm_balance').show();
                    break;
                case 'spin':
                    if (user.spin1 == 'true') {
                        spin1.play();
                        $('#slotspin1').attr('loop', 'loop');
                        $('#slotspin1').show();
                    }
                    if (user.spin2 == 'true') {
                        spin2.play();
                        $('#slotspin2').attr('loop', 'loop');
                        $('#slotspin2').show();
                    }
                    if (user.spin3 == 'true') {
                        spin3.play();
                        $('#slotspin3').attr('loop', 'loop');
                        $('#slotspin3').show();
                    }
                    if (checkbox2 == 1)
                        spin4.play();
                    $('#spin4').attr('loop', 'loop');
                    $('#slottitle').text('..SPINNING');
                    break;
                    case 'gang_stuff':
                        {
                           
                        }
                case 'slotspin':
                    if (user.spin1 == '1') {
                        if (user.spin1_pic1 == "100") {
                            $('#spin1_pic1').css('top', 85);
                            $('#spin1_pic1').css('left', 180);
                        }
                        if (user.spin1_pic1 == "20") {
                            $('#spin1_pic1').css('top', 85);
                            $('#spin1_pic1').css('left', 185);
                        }
                        if (user.spin1_pic1 == "50") {
                            $('#spin1_pic1').css('top', 97);
                            $('#spin1_pic1').css('left', 180);
                        }
                        if (user.spin1_pic1 == "10" || user.spin1_pic1 == "5" || user.spin1_pic1 == "1") //
                        {
                            $('#spin1_pic1').css('top', 100);
                            $('#spin1_pic1').css('left', 195);
                        }
                        if (user.spin1_pic2 == "20") {
                            $('#spin1_pic2').css('top', 185);
                            $('#spin1_pic2').css('left', 185);
                        }
                        if (user.spin1_pic2 == "100") {
                            $('#spin1_pic2').css('top', 185);
                            $('#spin1_pic2').css('left', 180);
                        }
                        if (user.spin1_pic2 == "50") {
                            $('#spin1_pic2').css('top', 195);
                            $('#spin1_pic2').css('left', 180);
                        }
                        if (user.spin1_pic2 == "10" || user.spin1_pic2 == "5" || user.spin1_pic2 == "1") //
                        {
                            $('#spin1_pic2').css('top', 200);
                            $('#spin1_pic2').css('left', 195);
                        }
                        if (user.spin1_pic3 == "20") {
                            $('#spin1_pic3').css('top', 280);
                            $('#spin1_pic3').css('left', 185);
                        }
                        if (user.spin1_pic3 == "100") {
                            $('#spin1_pic3').css('top', 280);
                            $('#spin1_pic3').css('left', 180);
                        }
                        if (user.spin1_pic3 == "50") {
                            $('#spin1_pic3').css('top', 290);
                            $('#spin1_pic3').css('left', 180);
                        }
                        if (user.spin1_pic3 == "10" || user.spin1_pic3 == "5" || user.spin1_pic3 == "1") //
                        {
                            $('#spin1_pic3').css('top', 295);
                            $('#spin1_pic3').css('left', 195);
                        }
                        $('#spin1_pic1').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/slot/' + user.spin1_pic1 + '.png)');
                        $('#spin1_pic2').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/slot/' + user.spin1_pic2 + '.png)');
                        $('#spin1_pic3').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/slot/' + user.spin1_pic3 + '.png)');
                        $('#slotspin1').hide();
                        spin1.pause();
                    }
                    if (user.spin2 == '1') {
                        if (user.spin2_pic1 == "100") {
                            $('#spin2_pic1').css('top', 85);
                            $('#spin2_pic1').css('left', 360);
                        }
                        if (user.spin2_pic1 == "20") {
                            $('#spin2_pic1').css('top', 85);
                            $('#spin2_pic1').css('left', 365);
                        }
                        if (user.spin2_pic1 == "50") {
                            $('#spin2_pic1').css('top', 97);
                            $('#spin2_pic1').css('left', 360);
                        }
                        if (user.spin2_pic1 == "10" || user.spin2_pic1 == "5" || user.spin2_pic1 == "1") //
                        {
                            $('#spin2_pic1').css('top', 100);
                            $('#spin2_pic1').css('left', 375);
                        }
                        if (user.spin2_pic2 == "20") {
                            $('#spin2_pic2').css('top', 185);
                            $('#spin2_pic2').css('left', 365);
                        }
                        if (user.spin2_pic2 == "100") {
                            $('#spin2_pic2').css('top', 185);
                            $('#spin2_pic2').css('left', 360);
                        }
                        if (user.spin2_pic2 == "50") {
                            $('#spin2_pic2').css('top', 195);
                            $('#spin2_pic2').css('left', 360);
                        }
                        if (user.spin2_pic2 == "10" || user.spin2_pic2 == "5" || user.spin2_pic2 == "1") //
                        {
                            $('#spin2_pic2').css('top', 200);
                            $('#spin2_pic2').css('left', 375);
                        }
                        if (user.spin2_pic3 == "20") {
                            $('#spin2_pic3').css('top', 280);
                            $('#spin2_pic3').css('left', 365);
                        }
                        if (user.spin2_pic3 == "100") {
                            $('#spin2_pic3').css('top', 280);
                            $('#spin2_pic3').css('left', 360);
                        }
                        if (user.spin2_pic3 == "50") {
                            $('#spin2_pic3').css('top', 290);
                            $('#spin2_pic3').css('left', 360);
                        }
                        if (user.spin2_pic3 == "10" || user.spin2_pic3 == "5" || user.spin2_pic3 == "1") //
                        {
                            $('#spin2_pic3').css('top', 295);
                            $('#spin2_pic3').css('left', 375);
                        }
                        $('#spin2_pic1').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/slot/' + user.spin2_pic1 + '.png)');
                        $('#spin2_pic2').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/slot/' + user.spin2_pic2 + '.png)');
                        $('#spin2_pic3').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/slot/' + user.spin2_pic3 + '.png)');
                        $('#slotspin2').hide();
                        spin2.pause();
                    }
                    if (user.spin3 == '1') {
                        if (user.spin3_pic1 == "100") {
                            $('#spin3_pic1').css('top', 85);
                            $('#spin3_pic1').css('left', 540);
                        }
                        if (user.spin3_pic1 == "20") {
                            $('#spin3_pic1').css('top', 85);
                            $('#spin3_pic1').css('left', 545);
                        }
                        if (user.spin3_pic1 == "50") {
                            $('#spin3_pic1').css('top', 97);
                            $('#spin3_pic1').css('left', 540);
                        }
                        if (user.spin3_pic1 == "10" || user.spin3_pic1 == "5" || user.spin3_pic1 == "1") //
                        {
                            $('#spin3_pic1').css('top', 100);
                            $('#spin3_pic1').css('left', 555);
                        }
                        if (user.spin3_pic2 == "20") {
                            $('#spin3_pic2').css('top', 185);
                            $('#spin3_pic2').css('left', 545);
                        }
                        if (user.spin3_pic2 == "100") {
                            $('#spin3_pic2').css('top', 185);
                            $('#spin3_pic2').css('left', 540);
                        }
                        if (user.spin3_pic2 == "50") {
                            $('#spin3_pic2').css('top', 195);
                            $('#spin3_pic2').css('left', 540);
                        }
                        if (user.spin3_pic2 == "10" || user.spin3_pic2 == "5" || user.spin3_pic2 == "1") //
                        {
                            $('#spin3_pic2').css('top', 200);
                            $('#spin3_pic2').css('left', 555);
                        }
                        if (user.spin3_pic3 == "20") {
                            $('#spin3_pic3').css('top', 280);
                            $('#spin3_pic3').css('left', 545);
                        }
                        if (user.spin3_pic3 == "100") {
                            $('#spin3_pic3').css('top', 280);
                            $('#spin3_pic3').css('left', 540);
                        }
                        if (user.spin3_pic3 == "50") {
                            $('#spin3_pic3').css('top', 290);
                            $('#spin3_pic3').css('left', 540);
                        }
                        if (user.spin3_pic3 == "10" || user.spin3_pic3 == "5" || user.spin3_pic3 == "1") //
                        {
                            $('#spin3_pic3').css('top', 295);
                            $('#spin3_pic3').css('left', 555);
                        }
                        $('#spin3_pic1').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/slot/' + user.spin3_pic1 + '.png)');
                        $('#spin3_pic2').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/slot/' + user.spin3_pic2 + '.png)');
                        $('#spin3_pic3').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/slot/' + user.spin3_pic3 + '.png)');
                        $('#slotspin3').hide();
                        spin3.pause();
                        spin4.pause();
                        if (user.win == '1') {
                            if (checkbox2 == 1)
                                spin6.play();
                            $('#slottitle').text('JACKPOT!!');
                        } else $('#slottitle').text('Better luck next time!');
                    }
                    if (checkbox2== 1)
                        spin5.play();
                    break;
                    case 'taxi':
                        $("#taxi-box").show();

                        
                        break;
                        case 'startwork':


                        break;

                case 'gang':
                    if (user.data == 'false')
                        $("#creategang").show();
                    else {
                        $("#creategang").hide();
                        $("#creategang-input").val('');
                        if (user.data == 'owner') {
                            $('#editcolor').show();
                            $('#gangleave').text('Delete Gang');
                        } else {
                            $('#editcolor').hide();
                            $('#gangleave').text('Leave Gang');
                        }
                        document.getElementById("gang_title3").innerHTML = user.level;
                        document.getElementById("gang_title4").innerHTML = user.gangname;
                        $('#gangxp_text').text(user.xptext);
                        $('#gangmembers').text(user.members);
                        $('#gangxp_inner').css({
                            width: ((user.xp1 / user.xp2) * 100) + '%'
                        }, 200);
                        document.getElementById("gang_rank").innerHTML = user.myrank;
                        document.getElementById("gang_kills").innerHTML = user.mykills;
                        document.getElementById("gang_hits").innerHTML = user.myhits;
                        document.getElementById("gang_arrests").innerHTML = user.myarrests;
                        document.getElementById("gang_jb").innerHTML = user.myjb;
                        document.getElementById("gang_cap").innerHTML = user.mycaptures;
                        $('#gang_look').css('content', 'url(' + look + user.look + '&direction=2&headonly=0)');
                        document.getElementById("gangrank").innerHTML = user.gangrank;
                        document.getElementById("gangkills").innerHTML = user.gangkills;
                        document.getElementById("ganghits").innerHTML = user.ganghits;
                        document.getElementById("gangarrests").innerHTML = user.gangarrests;
                        document.getElementById("gangjb").innerHTML = user.gangjb;
                        document.getElementById("gangcap").innerHTML = user.gangturfs;
                        $('#gangcolor1').css("background", user.color1);
                        $('#gangcolor2').css("background", user.color2);
                        $('#gcolor1_box').css("background", user.color1);
                        $('#gcolor2_box').css("background", user.color2);
                        $('#gang').show();
                    }
                    break;
                case 'gangrival':
                    if (user.ally1 != 'null') {
                        document.getElementById("ally1").innerHTML = user.ally1;
                        if (user.data == 'owner')
                            $('#allyx1').show();
                        else $('#allyx1').hide();
                    } else {
                        $('#ally1').text('');
                        $('#allyx1').hide();
                    }
                    if (user.ally2 != 'null') {
                        document.getElementById("ally2").innerHTML = user.ally2;
                        if (user.data == 'owner')
                            $('#allyx2').show();
                        else $('#allyx2').hide();
                    } else {
                        $('#ally2').text('');
                        $('#allyx2').hide();
                    }
                    if (user.rival1 != 'null') {
                        document.getElementById("rival1").innerHTML = user.rival1;
                        if (user.data == 'owner')
                            $('#rivalx1').show();
                        else $('#rivalx1').hide();
                    } else {
                        $('#rival1').text('');
                        $('#rivalx1').hide();
                    }
                    if (user.rival2 != 'null') {
                        document.getElementById("rival2").innerHTML = user.rival2;
                        if (user.data == 'owner')
                            $('#rivalx2').show();
                        else $('#rivalx2').hide();
                    } else {
                        $('#rival2').text('');
                        $('#rivalx2').hide();
                    }
                    $('#rival1').show();
                    $('#rival2').show();
                    $('#ally1').show();
                    $('#ally2').show();
                    $('#gangrival').show();
                    break;
                case 'ganginvite':
                    if (user.invite == 'true')
                        $('#invite_text').show();
                    else $('#rank_text').show();
                    break;
                case 'gangalert':
                    $('#gang_alert').show();
                    break;
                case 'ganghide':
                    $("#gang").hide();
                    $('#gangcolor').hide();
                    $('#leavegang').hide();
                    $('#gang2').hide();
                    $('#gangrival').hide();
                    $('#ally2').text('');
                    $('#ally1').text('');
                    $('#rival2').text('');
                    $('#rival1').text('');
                    $('#allyx2').hide();
                    $('#allyx1').hide();
                    $('#rivalx2').hide();
                    $('#rivalx1').hide();
                    break;
                case 'cancel_trade':
                    $('#tslot1').css('content', 'url()');
                    $('#tslot2').css('content', 'url()');
                    $('#tslot3').css('content', 'url()');
                    $('#tslot4').css('content', 'url()');
                    $('#tslot5').css('content', 'url()');
                    $('#tslot6').css('content', 'url()');
                    $('#tslot7').css('content', 'url()');
                    $('#tslot8').css('content', 'url()');
                    $('#tslot9').css('content', 'url()');
                    $('#tslot10').css('content', 'url()');
                    $('#tslot11').css('content', 'url()');
                    $('#tslot12').css('content', 'url()');
                    $('#twhp1').hide();
                    $('#twhp2').hide();
                    $('#twhp3').hide();
                    $('#twhp4').hide();
                    $('#twhp5').hide();
                    $('#twhp6').hide();
                    $('#twhp7').hide();
                    $('#twhp8').hide();
                    $('#twhp9').hide();
                    $('#twhp10').hide();
                    $('#twhp11').hide();
                    $('#twhp12').hide();
                    $('#tquantity1').hide();
                    $('#tquantity2').hide();
                    $('#tquantity3').hide();
                    $('#tquantity4').hide();
                    $('#tquantity5').hide();
                    $('#tquantity6').hide();
                    $('#tquantity7').hide();
                    $('#tquantity8').hide();
                    $('#tquantity9').hide();
                    $('#tquantity10').hide();
                    $('#tquantity11').hide();
                    $('#tquantity12').hide();
                    $('#addmoney').hide();
                    $('#itemwindow').animate({
                        height: '240px'
                    }, 500);
					$('#itemwindow_border').animate({
                        height: '238px'
                    }, 500);
                    break;
                case 'safe':
                    if (user.storage1 != '0')
                        $('#ss-1').text('Storage ID: ' + user.storage1 + '');
                    else
                        $('#ss-1').text('Empty');
                    if (user.storage2 != '0')
                        $('#ss-2').text('Storage ID: ' + user.storage2 + '');
                    else
                        $('#ss-2').text('Empty');
                    if (user.storage3 != '0')
                        $('#ss-3').text('Storage ID: ' + user.storage3 + '');
                    else
                        $('#ss-3').text('Empty');
                    if (user.storage4 != '0')
                        $('#ss-4').text('Storage ID: ' + user.storage4 + '');
                    else
                        $('#ss-4').text('Empty');
                    if (user.storage5 != '0')
                        $('#ss-5').text('Storage ID: ' + user.storage5 + '');
                    else
                        $('#ss-5').text('Empty');
                    $('#select_storage').show();
                    break;
                case 'storage':
                    if (user.bslot1 != 'null')
                        new placeItem(user.bslot1, user.quantity1, user.hp1, user.maxhp1, "#bslot1", "#bquantity1", "#bhp1", "#bh1");
                    else {
                        $("#bslot1").css('content', 'url()');
                        $('#bquantity1').hide();
                        $('#bhp1').hide();
                    }
                    if (user.bslot2 != 'null')
                        new placeItem(user.bslot2, user.quantity2, user.hp2, user.maxhp2, "#bslot2", "#bquantity2", "#bhp2", "#bh2");
                    else {
                        $("#bslot2").css('content', 'url()');
                        $('#bquantity2').hide();
                        $('#bhp2').hide();
                    }
                    if (user.bslot3 != 'null')
                        new placeItem(user.bslot3, user.quantity3, user.hp3, user.maxhp3, "#bslot3", "#bquantity3", "#bhp3", "#bh3");
                    else {
                        $("#bslot3").css('content', 'url()');
                        $('#bquantity3').hide();
                        $('#bhp3').hide();
                    }
                    if (user.bslot4 != 'null')
                        new placeItem(user.bslot4, user.quantity4, user.hp4, user.maxhp4, "#bslot4", "#bquantity4", "#bhp4", "#bh4");
                    else {
                        $("#bslot4").css('content', 'url()');
                        $('#bquantity4').hide();
                        $('#bhp4').hide();
                    }
                    if (user.bslot5 != 'null')
                        new placeItem(user.bslot5, user.quantity5, user.hp5, user.maxhp5, "#bslot5", "#bquantity5", "#bhp5", "#bh5");
                    else {
                        $("#bslot5").css('content', 'url()');
                        $('#bquantity5').hide();
                        $('#bhp5').hide();
                    }
                    if (user.bslot6 != 'null')
                        new placeItem(user.bslot6, user.quantity6, user.hp6, user.maxhp6, "#bslot6", "#bquantity6", "#bhp6", "#bh6");
                    else {
                        $("#bslot6").css('content', 'url()');
                        $('#bquantity6').hide();
                        $('#bhp6').hide();
                    }
                    if (user.bslot7 != 'null')
                        new placeItem(user.bslot7, user.quantity7, user.hp7, user.maxhp7, "#bslot7", "#bquantity7", "#bhp7", "#bh7");
                    else {
                        $("#bslot7").css('content', 'url()');
                        $('#bquantity7').hide();
                        $('#bhp7').hide();
                    }
                    if (user.bslot8 != 'null')
                        new placeItem(user.bslot8, user.quantity8, user.hp8, user.maxhp8, "#bslot8", "#bquantity8", "#bhp8", "#bh8");
                    else {
                        $("#bslot8").css('content', 'url()');
                        $('#bquantity8').hide();
                        $('#bhp8').hide();
                    }
                    if (user.bslot9 != 'null')
                        new placeItem(user.bslot9, user.quantity9, user.hp9, user.maxhp9, "#bslot9", "#bquantity9", "#bhp9", "#bh9");
                    else {
                        $("#bslot9").css('content', 'url()');
                        $('#bquantity9').hide();
                        $('#bhp9').hide();
                    }
                    if (user.bslot10 != 'null')
                        new placeItem(user.bslot10, user.quantity10, user.hp10, user.maxhp10, "#bslot10", "#bquantity10", "#bhp10", "#bh10");
                    else {
                        $("#bslot10").css('content', 'url()');
                        $('#bquantity10').hide();
                        $('#bhp10').hide();
                    }
                    if (user.bslot11 != 'null')
                        new placeItem(user.bslot11, user.quantity11, user.hp11, user.maxhp11, "#bslot11", "#bquantity11", "#bhp11", "#bh11");
                    else {
                        $("#bslot11").css('content', 'url()');
                        $('#bquantity11').hide();
                        $('#bhp11').hide();
                    }
                    if (user.bslot12 != 'null')
                        new placeItem(user.bslot12, user.quantity12, user.hp12, user.maxhp12, "#bslot12", "#bquantity12", "#bhp12", "#bh12");
                    else {
                        $("#bslot12").css('content', 'url()');
                        $('#bquantity12').hide();
                        $('#bhp12').hide();
                    }
					if (user.bypass == 'True'){
                    $('#storage_title').text('Storage ID: ' + user.id + '');
                    $('#storage_container').show();
					}
                    break;
                case 'ap':
                    $('#storage_container').hide();
                    $('#select_storage').hide();
                    $('#vault').hide();
                    $('#atm').hide();
                    break;
                case 'trade_equip':
                    new placeItem(user.item, user.quantity, user.hp, user.maxhp, "#tslot" + user.slot + "", "#tquantity" + user.slot + "", "#twhp" + user.slot + "", "#twh" + user.slot + "");
                    break;
                case 'trade_unequip':
                    if (user.quantity > 0)
                        $('#tquantity' + user.slot + '').text(user.quantity);
                    else {
                        $('#tslot' + user.slot + '').css('content', 'url()');
                        $('#twhp' + user.slot + '').hide();
                        $('#tquantity' + user.slot + '').hide();
                    }
                    break;
                case 'opentrade':
                    $('#wtrade_text2').text(user.name2);
                    $('#trade_btntext1').text('accept');
                    document.getElementById("trade_money1").innerHTML = '0';
                    document.getElementById("trade_money2").innerHTML = '0';
                    $('#arrow_1').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/inven/arrow_1.png)');
                    $('#arrow_2').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/inven/arrow_2.png)');
                    $('#item_window').show();
                    $('#itemwindow').animate({
                        height: '439px'
                    }, 500);
					$('#itemwindow_border').animate({
                        height: '437px'
                    }, 500);
                    openw = 1;
                    break;
                case 'accept_trade':
                    if (user.info == '1') {
                        $('#arrow_1').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/inven/greenarrow_1.png)');
                        $('#trade_btntext1').text('modify');
                    } else if (user.info == '3') {
                        $('#arrow_1').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/inven/arrow_1.png)');
                        $('#trade_btntext1').text('accept');
                    } else if (user.info == '2')
                        $('#arrow_2').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/inven/greenarrow_2.png)');
                    else if (user.info == '4')
                        $('#arrow_2').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/inven/arrow_2.png)');
                    else if (user.info == '5')
                        document.getElementById("trade_money1").innerHTML = user.amount;
                    else if (user.info == '6')
                        document.getElementById("trade_money2").innerHTML = user.amount;

                    break;
                case 'stats':
                    $('#hpbar_inner').animate({
                        width: ((user.health / user.maxhealth) * 100) + '%'
                    }, 200);
                    $('#engybar_inner').animate({
                        width: (user.energy) + '%'
                    }, 200);
                    $('#hungbar_inner').animate({
                        width: (user.hunger) + '%'
                    }, 200);
                    $('#xpbar_inner').animate({
                        width: ((user.xp / user.xpdue) * 100) + '%'
                    }, 200);
                    $('#hpbar_text').text((user.health) + '/' + (user.maxhealth));
                    $('#xpbar_text').text((user.xp1) + '/' + (user.xp2));
                    $('#engybar_text').text((user.energy) + '/100');
                    $('#hungbar_text').text((user.hunger) + '/100');
                    $('#level').text('Level ' + user.level);
                    $('#money').text(user.money);
                    $('#mylook').css('content', 'url(' + look + user.look + '&direction=2&headonly=0)');
                    $('#background').fadeIn();
                    $('#openw').show();
                  //  $('bbj-glove').fadeIn();
                    $('#work').fadeIn();
                    $('#stun').show();
                    $('#open-t').show();
                   
                 // $("#login_window").show();
                    //$('#chatbox').show();
                    $('#jobs').show();
                    $('#taxi').show();
                    $('#open-wl').show();
                    $('#open-m').show();
                    $('#open-g').show();
                    $('#open_s').show();
                    $('#gangicon').show();
                    $('#wl_icon').show();
                    $('#mystats').fadeIn();
                    /*color1 = user.color1;
                    color2 = user.color2;*/
                    new GlobalColor();
                    if (user.enable1 == '1'){
						$('#checkbox1').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/checkbox2.png)');
						checkbox1 = 1;
					}
                    if (user.enable2 == '1') {
                       $('#checkbox2').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/checkbox2.png)');
						checkbox2 = 1;
                    }
                    if (user.enable3 == '1') {
                      $('#checkbox3').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/checkbox2.png)');
						checkbox3 = 1;
                    } 
                    if (user.wslot1 != 'null')
                        new placeItem(user.wslot1, user.quantity1, user.whp1, user.maxhp1, "#slot1", "#quantity1", "#whp1", "#wh1");
                    else {
                        $("#slot1").css('content', 'url()');
                        $('#quantity1').hide();
                        $('#whp1').hide();
                    }
                    if (user.wslot2 != 'null')
                        new placeItem(user.wslot2, user.quantity2, user.whp2, user.maxhp2, "#slot2", "#quantity2", "#whp2", "#wh2");
                    else {
                        $("#slot2").css('content', 'url()');
                        $('#quantity2').hide();
                        $('#whp2').hide();
                    }
                    if (user.wslot3 != 'null')
                        new placeItem(user.wslot3, user.quantity3, user.whp3, user.maxhp3, "#slot3", "#quantity3", "#whp3", "#wh3");
                    else {
                        $("#slot3").css('content', 'url()');
                        $('#quantity3').hide();
                        $('#whp3').hide();
                    }
                    if (user.wslot4 != 'null')
                        new placeItem(user.wslot4, user.quantity4, user.whp4, user.maxhp4, "#slot4", "#quantity4", "#whp4", "#wh4");
                    else {
                        $("#slot4").css('content', 'url()');
                        $('#quantity4').hide();
                        $('#whp4').hide();
                    }
                    if (user.wslot5 != 'null')
                        new placeItem(user.wslot5, user.quantity5, user.whp5, user.maxhp5, "#slot5", "#quantity5", "#whp5", "#wh5");
                    else {
                        $("#slot5").css('content', 'url()');
                        $('#quantity5').hide();
                        $('#whp5').hide();
                    }
                    if (user.wslot6 != 'null')
                        new placeItem(user.wslot6, user.quantity6, user.whp6, user.maxhp6, "#slot6", "#quantity6", "#whp6", "#wh6");
                    else {
                        $("#slot6").css('content', 'url()');
                        $('#quantity6').hide();
                        $('#whp6').hide();
                    }
                    if (user.wslot7 != 'null')
                        new placeItem(user.wslot7, user.quantity7, user.whp7, user.maxhp7, "#slot7", "#quantity7", "#whp7", "#wh7");
                    else {
                        $("#slot7").css('content', 'url()');
                        $('#quantity7').hide();
                        $('#whp7').hide();
                    }
                    if (user.wslot8 != 'null')
                        new placeItem(user.wslot8, user.quantity8, user.whp8, user.maxhp8, "#slot8", "#quantity8", "#whp8", "#wh8");
                    else {
                        $("#slot8").css('content', 'url()');
                        $('#quantity8').hide();
                        $('#whp8').hide();
                    }
                    if (user.wslot9 != 'null')
                        new placeItem(user.wslot9, user.quantity9, user.whp9, user.maxhp9, "#slot9", "#quantity9", "#whp9", "#wh9");
                    else {
                        $("#slot9").css('content', 'url()');
                        $('#quantity9').hide();
                        $('#whp9').hide();
                    }
                    if (user.wslot10 != 'null')
                        new placeItem(user.wslot10, user.quantity10, user.whp10, user.maxhp10, "#slot10", "#quantity10", "#whp10", "#wh10");
                    else {
                        $("#slot10").css('content', 'url()');
                        $('#quantity10').hide();
                        $('#whp10').hide();
                    }
                    if (user.wslot11 != 'null')
                        new placeItem(user.wslot11, user.quantity11, user.whp11, user.maxhp10, "#slot11", "#quantity11", "#whp11", "#wh11");
                    else {
                        $("#slot11").css('content', 'url()');
                        $('#quantity11').hide();
                        $('#whp11').hide();
                    }
                    if (user.wslot12 != 'null')
                        new placeItem(user.wslot12, user.quantity12, user.whp12, user.maxhp12, "#slot12", "#quantity12", "#whp12", "#wh12");
                    else {
                        $("#slot12").css('content', 'url()');
                        $('#quantity12').hide();
                        $('#whp12').hide();
                    }
                    break;
                case 'iteminfo':
				$('#iteminfo').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/items_info/' + user.item + '_info.png)').show();
				$('#itemtitle').text(user.itemtitle).show();  
				$('#itemuse').text(user.itemuse).show();
				  if (isNotWep(user.item) == false && user.item != '' && user.item != 'skateboard') {
                        $('#miniheart').show();
                        $('#itemhp').text(user.itemhp);
                        $('#itemhp').show();
                    } else {
                        $('#miniheart').hide();
                        $('#itemhp').hide();
                    }				
                    break;
                case 'itemhp':
                    if (user.slot == 'true')
                        $('#eh1').animate({
                            height: ((user.hp / user.maxhp) * 100) + '%'
                        }, 200);
                    else $('#eh2').animate({
                        height: ((user.hp / user.maxhp) * 100) + '%'
                    }, 200);
			        if (user.circle == 1){
						$('#circle_hp1').animate({width: ((user.hp2 / user.maxhp) * 100) + '%'}, 200);
						new highlight(1);
					}
						else if (user.circle == 2){
                        $('#circle_hp2').animate({width: ((user.hp2 / user.maxhp) * 100) + '%'}, 200);
						new highlight(2);
						}
                    break;
                case 'equip':
                    if (user.slot == 'equip1'){
                        new placeItem(user.equip, '0', user.whp, user.maxhp, "#eslot1", "null", "#ehp1", "#eh1");
					}
                    else if (user.slot == 'null') {
                        $('#eslot1').css('content', 'url()');
                        $('#ehp1').hide();
                    }
                    if (user.slot == 'equip2') {
                        if (/kevlar/.test(user.equip)) {
                            $('#eslot2').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/items/' + user.equip + '.png)');
                            $('#eh2').animate({
                                height: ((user.whp / user.maxhp) * 100) + '%'
                            }, 200);
                            $('#ehp2').show()
                        }
                    } else if (user.slot == 'null') {
                        $('#eslot2').css('content', 'url()');
                        $('#ehp2').hide();
                    }
                    if (user.quantity == '0') {
                        $('#slot' + user.hide + '').css('content', 'url()');
                        $('#whp' + user.hide + '').hide();
                        $('#quantity' + user.hide + '').hide();
                    } else
                        $('#quantity' + user.hide + '').text(user.quantity);

                    break;
                case 'equip2':
                    if (user.hide == "e1") {
                        $('#eslot1').css('content', 'url()');
                        $('#ehp1').hide();
                    } else if (user.hide == "e2") {
                        $('#eslot2').css('content', 'url()');
                        $('#ehp2').hide();
						curslot2 = 'null';
                    }
                    if (user.showrest == 'true') {
                        $('#slot' + user.show + '').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/items/' + user.equip + '.png)');
                        if (isNotWep(user.equip)) {
                            $('#quantity' + user.show + '').text(user.quantity);
                            $('#quantity' + user.show + '').show();
                            $('#whp' + user.show + '').hide();
                        } else {
                            $('#wh' + user.show + '').animate({
                                height: ((user.whp / user.maxhp) * 100) + '%'
                            }, 200);
                            $('#whp' + user.show + '').show();
                        }
                    }
                    break;
                    case 'gangturf':
                            $('#info-message').fadeIn();
                        break;
                        case 'jailbreak':
                            {
                                $('#jailbreak').fadeIn();
                            }


					case 'roomjobinfo':
                      
					$("#roomdisplay_text").text(user.roomid);
					$("#room_display").fadeIn();
                    if (roomid == 21)
                    {
                        $('#info-message').fadeOut();
                    }
					if (user.job == 'none')
					{
					$("#room_display").animate({
                        top: '2px'
                    }, 200);
						$('#job_display').fadeOut();
					}
					else 
					{
						$('#jobdisplay_image').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/wsbadge/' + user.job + ')');
						   $("#room_display").animate({
                        top: '79px'
                    }, 200);
						$('#job_display').fadeIn();
					}
                   
					break;
                case 'targetstats':
                    $('#targetlook').hide();
                    $('#targethp_inner').animate({
                        width: ((user.health / user.maxhealth) * 100) + '%'
                    }, 200);
                    $('#targetengy_inner').animate({
                        width: (user.energy) + '%'
                    }, 200);
                    $('#targethung_inner').animate({
                        width: (user.hunger) + '%'
                    }, 200);
                    $('#targetxp_inner').animate({
                        width: ((user.xp / user.xpdue) * 100) + '%'
                    }, 200);
                    $('#targethp_text').text((user.health) + '/' + (user.maxhealth));
                    $('#targetxp_text').text((user.xp1) + '/' + (user.xp2));
                    $('#targetengy_text').text((user.energy) + '/100');
                    $('#targethung_text').text((user.hunger) + '/100');
                    $('#targetlevel').text('Level ' + user.level);
                   // $("#targetstat_border").css("background", "-webkit-linear-gradient(top," + user.color1 + " 0," + user.color2 + " 100%)");
                    $('#targetname').text(user.username);
                    $('#targetlook').css('content', 'url(' + look + user.look + '&direction=4&headonly=0)');
                    $('#lock').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/statsbar/lockopen.png)');
                    $('#target').fadeIn();
                    $('#targetlook').fadeIn();
                   
              
                    if (lock = true)
                        lock = false;
                    isbot = user.isbot;
                    break;
                case 'health':
                    $('#hpbar_inner').animate({
                        width: ((user.health / user.maxhealth) * 100) + '%'
                    }, 200);
                    $('#hpbar_text').text((user.health) + '/' + (user.maxhealth));
                    if (bleeding == true && user.health > 25) {
                        bleeding = false;
                        $('#hpbar_inner').css('background-color', '#d1312e');
                    }
                    break;
                case 'targethealth':
                    $('#targethp_inner').animate({
                        width: ((user.health / user.maxhealth) * 100) + '%'
                    }, 200);
                    $('#targethp_text').text((user.health) + '/' + (user.maxhealth));
                    break;
                case 'energy':
                    $('#engybar_inner').animate({
                        width: (user.energy) + '%'
                    }, 200);
                    $('#engybar_text').text((user.energy) + '/100');
                    break;
                case 'hunger':
                    $('#hungbar_inner').animate({
                        width: (user.hunger) + '%'
                    }, 200);
                    $('#hungbar_text').text((user.hunger) + '/100');
                    break;
                case 'targethunger':
                    $('#targethung_inner').animate({
                        width: (user.hunger) + '%'
                    }, 200);
                    $('#targethung_text').text((user.hunger) + '/100');
                    break;
                case 'targetenergy':
                    $('#targetengy_inner').animate({
                        width: (user.energy) + '%'
                    }, 200);
                    $('#targetengy_text').text((user.energy) + '/100');
                    break;
                case 'xp':
                    $('#xpbar_inner').animate({
                        width: ((user.xp / user.xpdue) * 100) + '%'
                    }, 200);
                    $('#xpbar_text').text((user.xp1) + '/' + (user.xp2));
                    break;
                case 'sidealert':
                    var msgtype;
                    var time = 4000;
                    if (user.evnt == 'xp')
                        document.getElementById("sidealert").innerHTML = 'You received ' + user.xpalert.fontcolor("#99ffff") + ' XP!';
                    else if (user.evnt == 'ko') {
                        msgtype = 'killed';
                        document.getElementById("sidealert").innerHTML = '' + user.name1 + ' ' + msgtype.fontcolor("red") + ' ' + user.name2 + '';
                    } else if (user.evnt == 'arrest') {
                        msgtype = 'arrested';
                        document.getElementById("sidealert").innerHTML = '' + user.name1 + ' ' + msgtype.fontcolor("#4dff4d") + ' ' + user.name2 + '';
                    } else if (user.evnt == 'discharge') {
                        msgtype = 'discharged';
                        document.getElementById("sidealert").innerHTML = '' + user.name1 + ' ' + msgtype.fontcolor("#33ccff") + ' ' + user.name2 + '';
                    } else if (user.evnt == 'hired')
                        document.getElementById("sidealert").innerHTML = '' + user.name1.fontcolor(user.color1) + ' hired ' + user.name2.fontcolor(user.color2) + ' as a ' + user.job + '';
                    else if (user.evnt == 'fired')
                        document.getElementById("sidealert").innerHTML = '' + user.name1.fontcolor(user.color1) + ' dismissed ' + user.name2.fontcolor(user.color2) + '';
                    else if (user.evnt == 'promote')
                        document.getElementById("sidealert").innerHTML = '' + user.name1.fontcolor(user.color1) + ' promoted ' + user.name2.fontcolor(user.color2) + ' to a ' + user.job + '';
                    else if (user.evnt == 'demote')
                        document.getElementById("sidealert").innerHTML = '' + user.name1.fontcolor(user.color1) + ' demoted' + user.name2.fontcolor(user.color2) + ' to a ' + user.job + '';
                    else if (user.evnt == 'sendhome')
                        document.getElementById("sidealert").innerHTML = '' + user.name1.fontcolor(user.color1) + ' sent ' + user.name2.fontcolor(user.color2) + ' home for ' + user.time + ' minutes';
                    else if (user.evnt == 'ticket')
                        document.getElementById("sidealert").innerHTML = '' + user.name1.fontcolor(user.color1) + ' fined ' + user.name2.fontcolor(user.color2) + ' ' + user.amount + ' dollars';
                    else if (user.evnt == 'online')
                        document.getElementById("sidealert").innerHTML = '' + user.name1.fontcolor(user.color) + ' is now online';
                    else if (user.evnt == 'offline')
                        document.getElementById("sidealert").innerHTML = '' + user.name1.fontcolor(user.color) + ' is now offline';
                    else if (user.evnt == 'account')
                        document.getElementById("sidealert").innerHTML = '' + user.name1.fontcolor(user.color) + ' opened a new account';
					else if (user.evnt == 'charge')
                        document.getElementById("sidealert").innerHTML = '' + user.name1.fontcolor(user.color1) + ' charged ' + user.name2.fontcolor(user.color2) + ' with ' + user.charge + '';
					else if (user.evnt == 'money')
					{
						if (user.add == 'true')
                        document.getElementById("sidealert").innerHTML = 'You received ' + user.amount.fontcolor("#5e9500") + ' dollars!';
					else document.getElementById("sidealert").innerHTML = 'You lost ' + user.amount.fontcolor("#5e9500") + ' dollars!';
					}

                    var myDiv = document.getElementById("sidealert");
                    var divClone = myDiv.cloneNode(true);
                    document.body.appendChild(divClone);
                    $(divClone).fadeIn();
                    $(divClone).animate({
                        opacity: 0,
                        height: 0,
                        marginTop: 0,
                        marginBottom: 0,
                        paddingTop: 0,
                        paddingBottom: 0
                    }, time);
                    break;
                
                case 'acceptitem':
                    if (user.info == 'cancel')
                        $('#acceptitem').hide();
                    else {
                        document.getElementById("accepttext").innerHTML = user.info;
                        $('#acceptitem').show();
                    }
                    break;
                case 'hungeralert':
				    $('#hungmeal').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/meals/meal'+user.meal+'.png)');
                    $('#hungalert').fadeIn();
                    if (checkbox2 == 1)
                        notify.play();
                    break;
				case 'nudge':
                    if (checkbox2 == 1)
                        notify.play();
                    break;
                case 'selectmeal':
                    $('#selectmeal').show();
                    break;
                case 'targetxp':
                    $('#targetxp_inner').animate({
                        width: ((user.xp / user.xpdue) * 100) + '%'
                    }, 200);
                    $('#targetxp_text').text((user.xp1) + '/' + (user.xp2));
                    break;
                case 'level':
                    $('#level').text('Level ' + user.level);
                    break;
                case 'targetlevel':
                    $('#targetlevel').text('Level ' + user.level);
                    break;
                case 'look':
                    $('#mylook').hide();
                    $('#mylook').css('content', 'url(' + look + user.look + '&direction=2&headonly=0)');
                    $('#mylook').fadeIn();
                    break;
                case 'targetlook':
                    $('#targetlook').hide();
                    $('#targetlook').css('content', 'url(' + look + user.look + '&direction=4&headonly=0)');
                    $('#targetlook').fadeIn();
                    break;
                case 'bleed':
                    if (user.bleed == 'true')
                        $('#hpbar_inner').css("background", "#ff0000");
                    else $('#hpbar_inner').css("background", "#560e15");
                    bleeding = true;
                    break;
                case 'copbadge':
                    if (user.copbadge == 'true') {
                        $('#workbadge').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/cop/911alert.png)');
                        $('#workbadge').fadeIn();
                    } else {
                        $('#workbadge').hide();
                        $('#police').hide();
						policecall = 0;
                    }
                    break;
                    case 'arena':
                        if (user.arena == 'true') {
                            $('#bbj-glove').css('content', 'url(' + RPurl + '/rp/red-glove.png)');
                            $('#bbj-glove').fadeIn();
                        } else {
                            $('#bbj-glove').fadeOut();
                        }

                        
                        break;
                    case 'jailbreak':
                        if (user.jailbreak == 'true') {
                            $('#jailbreak').fadeIn();
                        } else {
                            $('#jailbreak').hide();

                        }
                        break;
                case 'copalert':                   
                    if (checkbox2 == 1)
                        copbeep.play();
					if (copbeeping && copblink == 0){
						copblink = 1;
					}
                    else if (copblink == 0 && !copbeeping) {
                        copblink = 1;
                        new CopAlert();
						copbeeping = true;
                    }	
                    $('#callpage').text(user.pagestart + '/' + user.pageend);  					
                    break;
                case 'lock':
                    if (user.lock == 'false') {
                        $('#lock').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/statsbar/lockclosed.png)');
                        lock = true;
                    } else if (user.lock == 'true') {
                        $('#lock').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/statsbar/lockopen.png)');
                        lock = false;
                    }
                    break;
                case 'colorbox':
                    $('#color-name').text('' + username + '');
                    $('#color-name').css('color', user.color);
                    $('#colorbox').show();
                    break;
                case 'creategang':



                    break;
				case 'itemdrag':
				if (ItemDrag){
				$("#itemdrag").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/items/'+user.item+'.png)');					
                prevItem = user.slot;	
                ItemMove = user.item;	
				ItemType = user.itemtype;
				CurItem = false;	
				document.addEventListener('mousemove', mouseMov);	
				}
				else {
				document.removeEventListener('mousemove', mouseMov);
				ItemDrag = false;
				CurItem = true;
				$('#itemdrag').hide();
				}			
				break;
				case 'itemdrag2':
				if (user.itemtype == 'slot'){
				$('#slot'+ user.slot +'').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/items/'+user.item+'.png)');	
                if (isNotWep(user.item)) 
		        $('#quantity'+user.slot+'').show();	
                else $('#whp'+user.slot+'').show();	
				}
                else {
				$('#bslot'+ user.slot +'').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/items/'+user.item+'.png)');	
                if (isNotWep(user.item)) 
		        $('#bquantity'+user.slot+'').show();	
                else $('#bhp'+user.slot+'').show();	
				}					
				break;
				case 'itemdrag3':
				ItemDrag = false;
				CurItem = false;
				prevItem = '';
				document.removeEventListener('mousemove', mouseMov);
				$("#itemdrag").hide();
				break;
				case 'deleteitem':
				$('#deleteitem').show();
				break;
				case 'selectstun':
				$('#select_stun').show();
				break;
            }
        }
        ws.onclose = function() {
            console.log('connection closed!');
            window.location.replace("http://127.0.0.1/disconnect");

        }
        ws.onerror = function() {
            console.log('error detected!');
        }
    }
    function openCity(evt, cityName) {
        var i, tabcontent, tablinks;
        tabcontent = document.getElementsByClassName("tabcontent");
        for (i = 0; i < tabcontent.length; i++) {
          tabcontent[i].style.display = "none";
        }
        tablinks = document.getElementsByClassName("tablinks");
        for (i = 0; i < tablinks.length; i++) {
          tablinks[i].className = tablinks[i].className.replace(" active", "");
        }
        document.getElementById(cityName).style.display = "block";
        evt.currentTarget.className += " active";
      }
    //mouse hold
    $('div').on('mousedown mouseup', function mouseState(e) {
        var getName = $(this).attr('id');
        var div;
		//item drag
		 if ((/slot/.test(getName) && !/e/.test(getName) && !/tslot/.test(getName)) || ItemDrag){
			if (e.type == "mousedown"){				
			if (!ItemDrag)
			  ws.send('Eventitemdrag.Name' + username + '.Data' + getName + '.ExtraData.Token' + token);
		      ItemDrag = true;
			  }
            else if (e.type == "mouseup"){
				document.removeEventListener('mousemove', mouseMov);
				ItemDrag = false;
				CurItem = true;
				$('#itemdrag').hide();
			}
		 }
          if (/_header/.test(getName)) {
			  if (getName == 'sg_header')
                div = "#select_stun";

            else if (getName == 'trade_header')
                div = "#trade";
            else if (getName == 'accept_header')
                div = "#acceptitem";
            else if (getName == 'wl_header')
                div = "#wl";
            else if (getName == 'meal_header')
                div = "#selectmeal";
            else if (getName == 'color_header')
                div = "#colorbox";
            else if (getName == 'color_header2')
                div = "#colorbox2";
            else if (getName == 'storage_header')
                div = "#storage_container";
            else if (getName == 'item_header')
                div = "#item_window";
                else if (getName == 'gang_header')
                div = "#gang_window";
                else if (getName == 'taxi_header')
                div = "#taxi_window";
                else if (getName == 'login_header')
                div = "#login_window";
			else if (getName == 'police_header')
                div = "#police";
            else if (getName == 'settings_header')
                div = "#setting";
            else if (getName == 'creategang-header')
                div = '#creategang';
            else if (getName == 'ss_header')
                div = "#select_storage";
            else if (getName == 'atm_header')
                div = "#atm";
            else if (getName == 'panel_header')
                div = "#vault";
            else if (getName == 'cfh-header')
                div = "#cfh-box";
            else if (getName == 'stats_header')
                div = "#fullstats";
			else if (getName == 'delete_header')
                div = "#deleteitem";
			else if (getName == "addmoney_header")
                div = "#addmoney";
            if (e.type == "mousedown")
                $(div).draggable({
                    disabled: false
                });
            else if (e.type == "mouseup")
                $(div).draggable({
                    disabled: true
                });
            }
		    else if (getName == "halert") {
            div = "#halert";
            if (e.type == "mousedown")
                $(div).draggable({
                    disabled: false
                });
            else if (e.type == "mouseup")
                $(div).draggable({
                    disabled: true
                });
        } else if (/close/.test(getName) && !/panel/.test(getName)) {
            if (e.type == "mousedown")
                $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/closehold.png)');
            else if (e.type == "mouseup")
                $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/close.png)');
        }else if (getName == 'colorok'){
			if (e.type == "mousedown")
            $('#colorok').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/buttons/okbutton_hold.png)');
		    else if (e.type == "mouseup")
			$('#colorok').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/buttons/okbutton.png)');
		}else if (/btn/.test(getName) || /button/.test(getName)) {
            if (getName == 'btn')
               div = "#acceptbtn";
            else if (getName == 'btn2')
                div = "#declinebtn";
            else if (getName == 'tradebtn1')
               div = "#trade_btn1";
            else if (getName == 'tradebtn2')
               div = "#trade_btn2";
            else if (getName == 'spin_button')
                div = "#spin";
			else if (getName == 'deletebtn1')
               div = "#yes_btn"; 		   
			else if (getName == 'deletebtn2')
               div = "#no_btn";
			if (e.type == "mousedown")
                $(div).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/buttons/button_hold.png)');
			else if (e.type == "mouseup")
				  $(div).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/buttons/button.png)');
        }
    });
	//mouse hover
    $('div').mouseover(function(e) {
        var getName = $(this).attr('id');
		var div;
		if (CurItem){
			CurItem = false;
			if (prevItem == '')
			ws.send('Eventitemposition.Name' + username + '.Data' + getName + '.ExtraData.Token' + token);
		else prevItem = '';
		}
		if (getName == "box_text4")
			$(this).css("color", "red");
		else if (getName == "tbtn1")
		$("#trade_button1").css("background", "#20ae6f");
	    else if (getName == "tbtn2")
		$("#trade_button2").css("background", "#a8615b");
        else if (getName == "targetx")
            $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/statsbar/hoverx.png)');
        else if (/close/.test(getName) && getName != "panel_close")
            $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/closehover.png)');
        else if (/wslot/.test(getName) || /equip/.test(getName) || (/_sg_/.test(getName) && !/pic/.test(getName)))
            $(this).css("background", "#d8d8d8");
        else if (/slot/.test(getName) && !/w/.test(getName) && !/b/.test(getName) && !/_/.test(getName)) {
            if (/ts/.test(getName))
               ws.send('Eventiteminfo.Name' + username + '.Data1.ExtraData' + getName + '.Token' + token);
            else ws.send('Eventiteminfo.Name' + username + '.Data.ExtraData' + getName + '.Token' + token);
        } else if (getName == 'colorok')
            $('#colorok').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/buttons/okbutton_hover.png)');
        else if (/btn/.test(getName) || /button/.test(getName)) {			
			if (getName == 'btn')
               div = "#acceptbtn";
            else if (getName == 'btn2')
                div = "#declinebtn";
            else if (getName == 'tradebtn1')
               div = "#trade_btn1";
            else if (getName == 'tradebtn2')
               div = "#trade_btn2";
            else if (getName == 'spin_button')
                div = "#spin";
			else if (getName == 'deletebtn1')
               div = "#yes_btn"; 		   
			else if (getName == 'deletebtn2')
               div = "#no_btn";
			$(div).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/buttons/button_hover.png)')
        } else if (/atm_option/.test(getName)) {
            $(this).css('border', '2px solid #8febf5');
            if (getName == 'atm_option1')
                $("#atm_text1").css("color", '#8febf5');
            else if (getName == 'atm_option2')
                $("#atm_text2").css("color", '#8febf5');
            else if (getName == 'atm_option3')
                $("#atm_text3").css("color", '#8febf5');
            else if (getName == 'atm_option4') {
                $("#atm_input").css('border', '2px solid #8febf5');
                $("#atm_text4").css("color", '#8febf5');
                $("#atm_input").css("color", '#8febf5');
            }
        } else if (/atm_title/.test(getName) || getName == 'atm_balance' || getName == 'atm_back')
            $(this).css('color', '#8febf5');
        else if (getName == 'wl_icon') {
            $(this).css('left', 168);
            $(this).css('bottom', 15);

        } else if (getName == 'trashicon') {
            $(this).css('left', 389);
            $(this).css('bottom', 224);


            $('#gangalert').css('left', 145);
            $('#gangalert').css('bottom', 27);
            $('#gangalert_text').css('left', 153);
            $('#gangalert_text').css('bottom', 31);
        } 
		else if (getName == 'stat_border' || getName == 'targetstat_border' || getName == 'timer' || getName == 'job_display' || getName == 'room_display') {
			 $(this).css('background', "rgba(0,0,0,0.1)");			
		}
		else if (/stats_tab/.test(getName))
            $(this).css("background", '#eeeeee');
        else if (/stats_icon/.test(getName)) {
            if (getName == 'stats_icon1')
                $("#stats_tab1").css("background", '#eeeeee');
            else if (getName == 'stats_icon2')
                $("#stats_tab2").css("background", '#eeeeee');
            else if (getName == 'stats_icon3')
                $("#stats_tab3").css("background", '#eeeeee');
            else if (getName == 'stats_icon4')
                $("#stats_tab4").css("background", '#eeeeee');
            else if (getName == 'stats_icon5')
                $("#stats_tab5").css("background", '#eeeeee');
        }

    }).mouseout(function(e) { // out
        var getName = $(this).attr('id');		
		var div;
		if (getName == "box_text4")
			$(this).css("color", "black");
        else if (getName == "tbtn1")
		$("#trade_button1").css("background", "#136842");
	 else if (getName == "tbtn2")
		$("#trade_button2").css("background", "#643935");
		else if (getName == "targetx")
            $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/statsbar/x.png)');
        else if (/close/.test(getName) && getName != "panel_close")
            $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/close.png)');
        else if (/wslot/.test(getName) || /equip/.test(getName) || (/_sg_/.test(getName) && !/pic/.test(getName)))
            $(this).css("background", "#cacaca");
        else if (getName == 'colorok')
            $('#colorok').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/buttons/okbutton.png)');
       else if (/slot/.test(getName) && !/w/.test(getName) && !/b/.test(getName) && !/_/.test(getName)) {
            $('#iteminfo').hide();
            $('#itemtitle').hide();
            $('#itemhp').hide();
            $('#itemuse').hide();
            $('#miniheart').hide();
        }else if (/btn/.test(getName) || /button/.test(getName)) {
            if (getName == 'btn')
             div = "#acceptbtn";
            else if (getName == 'btn2')
                div = "#declinebtn";
            else if (getName == 'tradebtn1')
               div = "#trade_btn1";
            else if (getName == 'tradebtn2')
               div = "#trade_btn2";
            else if (getName == 'spin_button')
                div = "#spin";
			else if (getName == 'deletebtn1')
               div = "#yes_btn"; 		   
			else if (getName == 'deletebtn2')
               div = "#no_btn";
			$(div).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/buttons/button.png)')
        } else if (/atm_option/.test(getName)) {
            $(this).css('border', '2px solid #419687');
            if (getName == 'atm_option1')
                $("#atm_text1").css("color", '#419687');
            else if (getName == 'atm_option2')
                $("#atm_text2").css("color", '#419687');
            else if (getName == 'atm_option3')
                $("#atm_text3").css("color", '#419687');
            else if (getName == 'atm_option4') {
                $("#atm_input").css('border', '2px solid #419687');
                $("#atm_text4").css("color", '#419687');
                $("#atm_input").css("color", '#419687');
            }
        } else if (/atm_title/.test(getName) || getName == 'atm_balance' || getName == 'atm_back')
            $(this).css('color', '#419687');
        else if (getName == 'wl_icon') {
            $(this).css('left', 169);
            $(this).css('bottom', 14);

      
    
        } else if (getName == 'trashicon') {
            $(this).css('left', 390);
            $(this).css('bottom', 225);
        } else if (getName == 'gang_icon') {
            $('#gangicon').css('left', 132);
            $('#gangicon').css('bottom', 12);

            $('#gangalert').css('left', 146);
            $('#gangalert').css('bottom', 26);
            $('#gangalert_text').css('left', 154);
            $('#gangalert_text').css('bottom', 30);
        } else if (/stats_tab/.test(getName))
            $(this).css("background", '#F7F7F7');
        else if (/stats_icon/.test(getName)) {
            if (getName == 'stats_icon1')
                $("#stats_tab1").css("background", '#F7F7F7');
            else if (getName == 'stats_icon2')
                $("#stats_tab2").css("background", '#F7F7F7');
            else if (getName == 'stats_icon3')
                $("#stats_tab3").css("background", '#F7F7F7');
            else if (getName == 'stats_icon4')
                $("#stats_tab4").css("background", '#F7F7F7');
            else if (getName == 'stats_icon5')
                $("#stats_tab5").css("background", '#F7F7F7');			
        }
		else if (getName == 'stat_border' || getName == 'targetstat_border' || getName == 'timer' || getName == 'job_display' || getName == 'room_display') {
			 $(this).css('background', "rgba(0,0,0,0.2)");			
		}
    });

    //mouse click

    $('div').click(function(e) {
        if (e.button == 0) {
            var getName = $(this).attr('id');
			 if (/_sg_/.test(getName)) {
                ws.send('Eventselect_stun.Name' + username + '.Data.ExtraData' + getName + '.Token' + token);
				$('#select_stun').hide();
			}
            else if (getName == 'taxi_locations') {
               ws.send('EventTaxi.Name' + username + '.Data' + minutes + '.ExtraData' + seconds + '.Token' + token);
            }
			else if (getName == 'close_sg')
                $('#select_stun').hide();
            else if (getName == 'closeatm')
                $('#atm').hide();
            else if (getName == 'halert_close')
                $('#halert').hide();
            
            else if (getName == 'closeslot')
                $('#slotmachine').hide();
            else if (getName == 'closerival')
                $('#gangrival').hide();
            else if (getName == 'closehunger')
                $('#hungalert').hide();
            else if (getName == 'close_ss')
                $('#select_storage').hide();
            else if (getName == 'box_text4')
                ws.send('Eventopencolor.Name' + username + '.Data.ExtraData.Token' + token);
            else if (getName == 'box_text5')
                $('#colorbox2').show();
            else if (getName == 'closestats')
                $("#fullstats").hide();
            else if (getName == 'color_default') {
                color1 = '#046e7c';
                color2 = '#02353c';
                new GlobalColor();
            }

            
            else if (getName == ('open-m')) {
                $("#marcobox").fadeIn();
            }
            
            else if (getName == ('open-g') && opengang == false)
            {
                opengang = true;
                $("#gang_window").fadeIn();

            // ALREADY IN GANG
            if (user.gangname == '') {

             // CREATE NEW GANG
               $("#creategang").fadeIn(); // Fade in Gang Box
               var input = $("#creategang-input").val(''); //  value of gang input
               opengang = true;
            }
            else {
                $('#showgang').show();
            }



               
              



              

               
            }
           
           
            else if (getName == ('open-g') && opengang == true)
            {
                $("#creategang").fadeOut();
                opengang = false;
            }
            else if (getName == ('taxi') && taxiwindow == false)
            {
               // $("#taxi-box").fadeIn();
                $("#taxi_window").show();
                taxiwindow = true;
                
            }
            else if (getName == ('taxi') && taxiwindow == true)
            {
                $("#taxi-box").fadeOut();
                taxiwindow = false;
            }
            else if (getName == ('open-t') && opencfh == false) {
                $("#cfh-box").fadeIn();
                opencfh = true;
            }
            else if (getName == ('open-g') && opencfh == false) {
                $("#gang_window").fadeIn();
                opencfh = true;
            }
            else if (getName == ('open-t') && opencfh == true) {
                $('#cfh-box').fadeOut();
                opencfh = false;
            }
            else if (getName == 'cfh-box-close' && opencfh == true)
            {
                $('#cfh-box').fadeOut();
                opencfh = false;
            }
            else if (getName == 'taxi-box-close' && taxiwindow == true)
            {
                $('#taxi-box').fadeOut();
                taxiwindow = false;
            }
         
            else if (getName == 'creategang-box-close' && opengang == true)
            {
                $('#creategang').fadeOut();
                opengang = false;
            }
            
			else if (getName == 'open-wl'){

            if (wl == false)
                ws.send('Eventwl.Name' + username + '.Datatrue.ExtraData.Token' + token);
            else {
                $('#wl').show();
                wl = show;
                ws.send('Eventwl.Name' + username + '.Datafalse.ExtraData.Token' + token);
            }
            }
			else if (getName == 'close_wl'){
            $('#wl').hide();
			ws.send('Eventwl.Name' + username + '.Datafalse.ExtraData.Token' + token);
            wl = false;
			}
			else if (getName == 'save_color') {
                $("#colorbox2").hide();
                ws.send('Eventglobalcolor.Name' + username + '.Data' + color1 + '.ExtraData' + color2 + '.Token' + token);
            } else if (/stat_button/.test(getName) && !/target/.test(getName)) {
                if (stat_arrow == false) {
                    stat_arrow = true;
                    $("#stat_button").animate({left: '187px'}, 200);
					$('#hpbar').show();					
                    $('#engybar').show();
					$("#hpbar").animate({width: '120px'}, 200);
					$("#engybar").animate({width: '120px'}, 200);
					$("#targethp").animate({left: '200px'}, 200);
					$('#stat_button').css('transform','rotate(90deg)');					
					$("#targetstat_border").animate({left: '197px'}, 200);
					$("#targetx").animate({left: '251px'}, 200);
					$("#lock").animate({left: '254px'}, 200);
					$("#targetname").animate({left: '164px'}, 200);
                } else {
                    stat_arrow = false;
                    $("#stat_button").animate({left: '63px'}, 200);
					$("#hpbar").animate({width: '1px'}, 200).hide();
					$("#engybar").animate({width: '1px'}, 200).hide();
					$('#stat_button').css('transform','rotate(270deg)');
					$("#targetstat_border").animate({left: '75px'}, 200);
					$("#targethp").animate({left: '78px'}, 200);
				    $("#targetx").animate({left: '129px'}, 200);
					$("#lock").animate({left: '132px'}, 200);
					$("#targetname").animate({left: '42px'}, 200);
                }
			}
            else if (getName == 'taxi_close') {
                $('#taxi_window').hide();
                taxiwindow = false;
            }
            else if (getName == 'gang_close') {
                $('#gang_window').hide();
                gangwindow = false;
            }
             else if (getName == 'trade_close') {
                $('#trade').hide();
                ws.send('Eventtradeclose.Name' + username + '.Data.ExtraData.Token' + token);
            } else if (getName == 'closemeal') {
                $('#selectmeal').hide();
                ws.send('Eventselectmeal.Name' + username + '.Data.ExtraDatafalse');
            } else if (getName == 'closeitem') {
                openw = 0;
                $('#item_window').hide();
                ws.send('Eventcancel_trade.Name' + username + '.Data.ExtraData.Token' + token);
                trash = false;
                $('#trashicon').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/inven/trash.png)');
            } else if (getName == 'targetx') {
                $('#target').hide();
                if (lock == true)
                    lock = false;
                ws.send('Eventclosetarget.Name' + username + '.Datafalse.ExtraData' + isbot + '.Token' + token);
            } else if (getName == 'lock') {
                if (lock == false) {
                    $('#lock').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/statsbar/lockclosed.png)');
                    lock = true;
                    ws.send('Eventlock.Name' + username + '.Datatrue.ExtraData' + isbot + '.Token' + token);
                } else if (lock == true) {
                    $('#lock').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/statsbar/lockopen.png)');
					
                    lock = false;
                    ws.send('Eventlock.Name' + username + '.Datafalse.ExtraData' + isbot + '.Token' + token);
                }
            } else if (/ss-/.test(getName)) {
                ws.send('Eventstorage.Name' + username + '.Data.ExtraData' + getName + '.Token' + token);
                if ($(this).text() == 'Empty')
                    $('#storage_container').hide();
            } else if (/meal_/.test(getName) && getName != 'meal_header') {
                $('#selectmeal').hide();
                ws.send('Eventselectmeal.Name' + username + '.Data' + getName + '.ExtraDatatrue.Token' + token);
            } else if (getName == 'close_storage') {
                ws.send('Eventclose_storage.Name' + username + '.Datafalse.ExtraData.Token' + token);
                $("#storage_container").hide();
            } else if (getName == 'open_s') {
                if (settings == 1) {
                    settings = 0;
                    $("#setting").hide();
                } else {
                    settings = 1;
                    $("#setting").show();
                }
            } else if (getName == 'colorok') {
                var x = $('#color-name').css('color');
                hexc(x);
                console.log('' + color + '')
                ws.send('Eventnamecolor.Name' + username + '.Data' + color + '.ExtraData.Token' + token);
                $('#colorbox').hide();
            } else if (getName == 'close_settings') {
                settings = 0;
                $('#setting').hide();
            } else if (getName == 'openw') {
                if (openw == 0) {
                    $("#item_window").fadeIn();
                    openw = 1;
                } else {
                    $("#item_window").fadeOut();
                    ws.send('Eventtrash.Name' + username + '.Data.ExtraDatafalse.Token' + token);
                    trash = false;
                    $("#trashicon").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/inven/trash.png)');
                    openw = 0;
                }
         
            } else if (getName == 'trashicon') {
                if (trash == false) {
                    trash = true;
                    ws.send('Eventtrash.Name' + username + '.Data.ExtraDatatrue.Token' + token);
                    $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/inven/clicktrash.png)');
                } else {
                    trash = false;
                    ws.send('Eventtrash.Name' + username + '.Data.ExtraDatafalse.Token' + token);
                    $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/inven/trash.png)');
                }
            } else if (getName == 'atm_back') {
                $('#atm_back').hide();
                $('#atm_balance').hide();
                $('#atm_title2').hide();
                $('#atm_option4').hide();
                $('#atm_title').show();
                $('#atm_option1').show();
                $('#atm_option2').show();
                $('#atm_option3').show();
                $('#atm').show();
                $('#atm_input').val('');
            } else if (getName == 'atm_option1' || getName == 'atm_option2' || getName == 'atm_text1' || getName == 'atm_text2') {
                $('#atm_title').hide();
                $('#atm_option1').hide();
                $('#atm_option2').hide();
                $('#atm_option3').hide();
                $('#atm_balance').hide();
                $('#atm_title2').show();
                $('#atm_back').show();
                $('#atm_option4').show();
                if (getName == 'atm_option1' || getName == 'atm_text1') {
                    atm = 'withdraw';
                    $('#atm_title2').text('WITHDRAW');
                } else {
                    atm = 'deposit';
                    $('#atm_title2').text('DEPOSIT');
                }
            } else if (getName == 'atm_option3' || getName == 'atm_text3')
                ws.send('Eventatm_balance.Name' + username + '.Data.ExtraData.Token' + token);
            else if (getName == 'closecall'){
                $('#police').hide();
				policecall = 0;
			}
            else if (getName == '#jobs') {      
                ws.send('Eventstartwork.Name' + username + '.Datatrue.ExtraData.Token' + token);
         }
            else if (getName == 'closemarco')
                $("#marcobox").hide();
			  else if (getName == 'smname')
                ws.send('Eventstatsrequest.Name' + username + '.Data' + $("#smname").text() + '.ExtraDatasoulmate.Token' + token);
            else if (getName == 'mylook')
                ws.send('Eventstatsrequest.Name' + username + '.Datatrue.ExtraDatafalse.Token' + token);
            else if (getName == 'targetlook')
                ws.send('Eventstatsrequest.Name' + username + '.Datafalse.ExtraData' + isbot + '.Token' + token);
            else if (getName == 'callerlook')
                ws.send('Eventstatsrequest.Name' + username + '.Data.ExtraDatafalse.Token' + token);
			else if (getName == 'wl_look')
                ws.send('Eventwl_profile.Name' + username + '.Data.ExtraData.Token' + token);
            else if (getName == 'callerroom')
                ws.send('Event911room.Name' + username + '.Data.ExtraData.Token' + token);
			else if (getName == 'wl_next')
                ws.send('Eventwl_next.Name' + username + '.Data.ExtraData.Token' + token);
			else if (getName == 'wl_clear')
                ws.send('Eventwl_clear.Name' + username + '.Data.ExtraData.Token' + token);
			else if (getName == 'wl_back')
                ws.send('Eventwl_back.Name' + username + '.Data.ExtraData.Token' + token);
            else if (getName == 'callnext'){
                ws.send('Event911next.Name' + username + '.Data.ExtraData.Token' + token);
				      copblink = 0;
				$('#workbadge').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/cop/911alert.png)');
			}
			else if (getName == 'clearcall'){
                ws.send('Event911clear.Name' + username + '.Data.ExtraData.Token' + token);
				      copblink = 0;
				$('#workbadge').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/cop/911alert.png)');
			}
            else if (getName == 'callback'){
                ws.send('Event911back.Name' + username + '.Data.ExtraData.Token' + token);
				      copblink = 0;
				$('#workbadge').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/cop/911alert.png)');
			}
            else if (getName == 'workbadge') {
                ws.send('Event911.Name' + username + '.Data.ExtraData.Token' + token);
                copblink = 0;
				$('#workbadge').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/cop/911alert.png)');
            } else if (getName == 'closecolor')
                $('#colorbox').hide();
            else if (getName == 'closecolor2')
                $('#colorbox2').hide();
            else if (/slot/.test(getName) || /quantity/.test(getName)) {
                if (/tslot/.test(getName) || /tquantity/.test(getName))
                    ws.send('Eventunequip_trade.Name' + username + '.Data.ExtraData' + getName + '.Token' + token);
                else if (/bslot/.test(getName) || /bquantity/.test(getName))
                    ws.send('Eventunequip_storage.Name' + username + '.Data.ExtraData' + getName + '.Token' + token);
                else ws.send('Eventequip.Name' + username + '.Data.ExtraData' + getName + '.Token' + token);
            } else if (!/trade_btntext/.test(getName) && (/trade_btn/.test(getName) || /trade_text/.test(getName))) {
                $('#trade').hide();
                ws.send('Eventtrade.Name' + username + '.Data.ExtraData' + getName + '.Token' + token);
            } else if (getName == 'acceptbtn' || getName == 'btntext') {
                ws.send('Eventacceptitem.Name' + username + '.Data1.ExtraData.Token' + token);
                $('#acceptitem').hide();
            } else if (getName == 'declinebtn' || getName == 'btntext2') {
                ws.send('Eventacceptitem.Name' + username + '.Data.ExtraData.Token' + token);
                $('#acceptitem').hide();
            } else if (getName == 'closeaccept') {
                ws.send('Eventacceptitem.Name' + username + '.Data.ExtraData.Token' + token);
                $('#acceptitem').hide();
            } else if (getName == 'closedelete') {
                ws.send('Eventdeleteitem.Name' + username + '.Data.ExtraDatano.Token' + token);
                $('#deleteitem').hide();
		    } else if (getName == 'yes_btn' || getName == 'delete_btntext1') {
                    ws.send('Eventdeleteitem.Name' + username + '.Data.ExtraDatayes.Token' + token);
                $('#deleteitem').hide();
			} else if (getName == 'no_btn' || getName == 'delete_btntext2') {
                    ws.send('Eventdeleteitem.Name' + username + '.Data.ExtraDatano.Token' + token);
                $('#deleteitem').hide();
            } else if (getName == 'trade_btntext1' || getName == 'trade_button1')
                ws.send('Eventaccept_trade.Name' + username + '.Data.ExtraData.Token' + token);
            else if (getName == 'trade_btntext2' || getName == 'trade_button2')
                ws.send('Eventcancel_trade.Name' + username + '.Data.ExtraData.Token' + token);
            else if (getName == 'panel_close') {
                $('#panel_input').val('');
                $('#vault').hide();
            } else if (getName == 'close_money') {
                $('#money_input').val('');
                $('#addmoney').hide();
            } else if (getName == 'trade_coin1' || getName == 'trade_money1')
                $('#addmoney').show();
            else if (getName == 'spin' || getName == 'spin_text')
                ws.send('Eventspin.Name' + username + '.Data.ExtraData.Token' + token);
            else if (getName == 'stats_tab1' || getName == 'stats_icon1') {
                $('#stats_tab1').hide();
                $('#stats_tab2').show();
                $('#stats_tab3').show();
                $('#stats_tab4').show();
                $('#stats_tab5').show();
                $('#usertab').show();
                $('#jobtab').hide();
                $('#gangtab').hide();
                $('#biotab').hide();
                $('#pettab').hide();
				if (soulmate.length > 0){
				    $("#statname").css({'width': 'auto'});
					var position = $("#statname").position();	
                    var width = $("#statname").width();	
					if (width > 288)
						{	
                            $("#smname").css({'left': position.left + width - 280});					
						    $("#smname").css({'top': '100'});
						}						
						else {
							$("#smname").css({'left': position.left + width + 4});
							$("#smname").css({'top': '83'});
						}	
					document.getElementById("smname").innerHTML = soulmate;					
					}
				   
            } else if (getName == 'stats_tab2' || getName == 'stats_icon2') {
                $('#stats_tab2').hide();
                $('#stats_tab1').show();
                $('#stats_tab3').show();
                $('#stats_tab4').show();
                $('#stats_tab5').show();
                $('#jobtab').show();
                $('#gangtab').hide();
                $('#biotab').hide();
                $('#pettab').hide();
                $('#usertab').hide();
            } else if (getName == 'stats_tab3' || getName == 'stats_icon3') {
                $('#stats_tab3').hide();
                $('#stats_tab1').show();
                $('#stats_tab2').show();
                $('#stats_tab4').show();
                $('#stats_tab5').show();
                $('#jobtab').hide();
                $('#gangtab').show();
                $('#biotab').hide();
                $('#pettab').hide();
                $('#usertab').hide();
            } else if (getName == 'stats_tab4' || getName == 'stats_icon4') {
                $('#stats_tab4').hide();
                $('#stats_tab1').show();
                $('#stats_tab3').show();
                $('#stats_tab2').show();
                $('#stats_tab5').show();
                $('#jobtab').hide();
                $('#gangtab').hide();
                $('#pettab').show();
                $('#biotab').hide();
                $('#usertab').hide();
            } else if (getName == 'stats_tab5' || getName == 'stats_icon5') {
                $('#stats_tab5').hide();
                $('#stats_tab1').show();
                $('#stats_tab3').show();
                $('#stats_tab4').show();
                $('#stats_tab1').show();
                $('#usertab').hide();
                $('#jobtab').hide();
                $('#gangtab').hide();
                $('#biotab').show();
                $('#pettab').hide();
            }else if (getName == 'editbio')
			{
				$('#editbio').hide();
				$('#bio').hide();
				$('#checkbio').show();
				$('#cancelbio').show();               
				var bio = $("#bio").text();
				$("#bio_input").val(bio + '');
				$("#bio_input").show();
			}
			else if (getName == 'cancelbio')
			{
				$('#editbio').show();
				$('#bio').show();
				$('#checkbio').hide();
				$('#cancelbio').hide();
				$("#bio_input").val('');
                $("#bio_input").hide();
			}
            else if (getName == 'checkbio')
			{
				var input = $("#bio_input").val();
				var bio = input.replace("<", "").replace(">", "");
                ws.send('Eventbio.Name' + username + '.Data.ExtraData' + bio + '.Token' + token);
			}
            if (/checkbox/.test(getName))
                return true;
            else return false;
        }
    });

    // Taxi

    $('#location1').click(function(e) {  
       ws.send('Eventcommunityavenue.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi_window").hide();
       taxiwindow = false; 
    })
    $('#location2').click(function(e) {  
        ws.send('Eventsouthernpark.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location3').click(function(e) {  
        ws.send('Eventpeacekeepersstreet.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location4').click(function(e) {  
        ws.send('Eventgatewaystreet.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location5').click(function(e) {  
        ws.send('Eventslurproad.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location6').click(function(e) {  
        ws.send('Eventwealthboulevard.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location7').click(function(e) {  
        ws.send('Eventlongstreet.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location8').click(function(e) {  
        ws.send('Eventeventstreet.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location9').click(function(e) {  
        ws.send('Eventmuseumroad.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location10').click(function(e) {  
        ws.send('Eventfurryroad.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location11').click(function(e) {  
        ws.send('Eventstardrive.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location12').click(function(e) {  
        ws.send('Eventrollieroad.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location13').click(function(e) {  
        ws.send('Eventfashionway.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location14').click(function(e) {  
        ws.send('Eventarmoryroad.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })
     $('#location15').click(function(e) {  
        ws.send('Eventmercyroad.Name' + username + '.Datatrue.ExtraData.Token' + token);
        $("#taxi_window").hide();
        taxiwindow = false; 
     })



    //


	$('#taxi-locations-mercy').click(function(e) {
        // Fwd to mercy
        
       ws.send('Eventtaxi.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi-box").hide();
       taxiwindow = false;

        
    })
    $('#bbj-glove').click(function(e) {
        // Fwd to mercy
        
       ws.send('Eventbbj-arena.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi-box").hide();
       taxiwindow = false;
        
    })
    $('#taxi-locations-outside-events').click(function(e) {
        // Fwd to mercy
        
       ws.send('Eventtaxieventouside.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi-box").hide();
       taxiwindow = false;
        
    })
    $('#jailbreak-button').click(function(e) {
        // Fwd to bbj
       ws.send('Eventjailbreak.Name' + username + '.Datatrue.ExtraData.Token' + token);
       
        
    })
   
    $('#info-button').click(function(e) {
        $('#info-message').fadeOut();
        info_message = false;
    })
    $('#creategang-button').click(function(e) {
        ws.send('Eventgangcreate.Name' + username + '.Data.ExtraData.Token' + token);
     })

    

    $('#taxi-locations-bbj').click(function(e) {
        // Fwd to bbj
       ws.send('Eventtaxibbj.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi-box").hide();
       taxiwindow = false;
        
    })
    $('#taxi-locations-bbj-outside').click(function(e) {
        // Fwd to bbj-outside
       ws.send('Eventtaxibbjoutside.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi-box").hide();
       taxiwindow = false;
        
    })
    $('#taxi-locations-hsbc').click(function(e) {
        // Fwd to bbj-outside
       ws.send('Eventtaxihsbc.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi-box").hide();
       taxiwindow = false;
        
    })
    $('#taxi-locations-nypd').click(function(e) {
        // Fwd to bbj-outside
       ws.send('Eventtaxinypd.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi-box").hide();
       taxiwindow = false;

        
    })
    $('#info-button').click(function(e) {
        // Fwd to bbj-outside
       ws.send('Eventgangclaim.Name' + username + '.Datatrue.ExtraData.Token' + token);

                            new startTimer(Minutes, display, user.seconds);
                            istimer = true;
        
    })
    $('#stun').click(function(e) {
        // Fwd to bbj-outside
       ws.send('Eventstun.Name' + username + '.Datatrue.ExtraData.Token' + token);
        
    })
    $('#taxi-locations-nypd-outside').click(function(e) {
        // Fwd to bbj-outside
       ws.send('Eventtaxinypdoutside.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi-box").hide();
       taxiwindow = false;
        
    })
    $('#taxi-locations-mercy-outside').click(function(e) {
        // Fwd to mercy-outside
       ws.send('Eventtaximercyoutside.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi-box").hide();
       taxiwindow = false;
        
    })
    $('#taxi-locations-hsbc-outside').click(function(e) {
        // Fwd to mercy-outside
       ws.send('Eventtaxihsbcoutside.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi-box").hide();
       taxiwindow = false;
        
    })
    $('#taxi-locations-flex').click(function(e) {
        // Fwd to mercy-outside
       ws.send('Eventtaxiflex.Name' + username + '.Datatrue.ExtraData.Token' + token);
       $("#taxi-box").hide();
       taxiwindow = false;
        
    })
    $('#jobs').click(function(e) {
        // Fwd to room
        if (working == false)
        {
        ws.send('Eventstartwork.Name' + username + '.Datatrue.ExtraData.Token' + token);
        working = true;
       

        }
        else
        {
            ws.send('Eventstopwork.Name' + username + '.Datatrue.ExtraData.Token' + token);
            working = false;
        }
        
    })
	//check box
    $('#checkbox1').click(function(e) {
        if (checkbox1 == 0){
           $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/checkbox2.png)');
			checkbox1 = 1;
			ws.send('Eventenable.Name' + username + '.Data1.ExtraData1.Token' + token);
		}
        else{
			 $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/checkbox1.png)');
			checkbox1 = 0;
            ws.send('Eventenable.Name' + username + '.Data.ExtraData1.Token' + token);
		}
    });
    $('#checkbox2').click(function(e) {
		 if (checkbox2 == 0){
           $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/checkbox2.png)');
			checkbox2 = 1;
			ws.send('Eventenable.Name' + username + '.Data1.ExtraData2.Token' + token);
		}
        else{
			 $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/checkbox1.png)');
			checkbox2 = 0;
           ws.send('Eventenable.Name' + username + '.Data.ExtraData2.Token' + token);
		}
    });
   $('#checkbox3').click(function(e) {
		 if (checkbox3 == 0){
           $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/checkbox2.png)');
			checkbox3 = 1;
			ws.send('Eventenable.Name' + username + '.Data1.ExtraData2.Token' + token);
		}
        else{
			 $(this).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/checkbox1.png)');
			checkbox3 = 0;
           ws.send('Eventenable.Name' + username + '.Data.ExtraData2.Token' + token);
		}
    });

    //key Press
    $('#atm_input').keypress(function(e) {
        if (e.which == 13) {
            var input = $("#atm_input").val();
		    var value = input.replace("<", "").replace(">", "");
            ws.send('Eventatm.Name' + username + '.Data' + atm + '.ExtraData' + value + '.Token' + token);
            $('#atm_input').val('');
        }
    });
    $('#money_input').keypress(function(e) {
        if (e.which == 13) {
            var input = $("#money_input").val();
			var value = input.replace("<", "").replace(">", "");
            ws.send('Eventadd_money.Name' + username + '.Data.ExtraData' + value + '.Token' + token);
            $('#money_input').val('');
            $('#addmoney').hide();
        }
    });
    $('#panel_input').keypress(function(e) {
        if (e.which == 13) {
            var input = $("#panel_input").val();
			var value = input.replace("<", "").replace(">", "");
            ws.send('Eventvault.Name' + username + '.Data.ExtraData' + value + '.Token' + token);
            $('#panel_input').val('');
        }
    });

    $("#gang_icon").click(function(e) {
        if (e.button == 0) {
            $('#invite_text').hide();
            $('#rank_text').hide();
            $('#gang_alert').hide();
            if (gangwindow == false) {
                ws.send('Eventgang.Name' + username + '.Data.ExtraData.Token' + token);
                gangwindow = true;
            } else {
                $("#cgang").hide();
                $("#gang").hide();
                $('#gangcolor').hide();
                $('#leavegang').hide();
                $('#gang2').hide();
                $('#gangrival').hide();
                gangwindow = false;
            }
            return false;
        }
    });

	// color box
    function hexc(colorval) {
        var parts = colorval.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
        delete(parts[0]);
        for (var i = 1; i <= 3; ++i) {
            parts[i] = parseInt(parts[i]).toString(16);
            if (parts[i].length == 1) parts[i] = '0' + parts[i];
        }
        color = '#' + parts.join('');
    }   
    $('#color-picker').iris({
        hide: false,
        palettes: true,
        change: function(event, ui) {
            $("#color-name").css('color', ui.color.toString());
        }
    });
    $('#color-picker2').iris({
        hide: false,
        palettes: true,
        change: function(event, ui) {
            color1 = ui.color.toString();
            new GlobalColor();
        }
    });
    $('#color-picker3').iris({
        hide: false,
        palettes: true,
        change: function(event, ui) {
            color2 = ui.color.toString();
            new GlobalColor();
        }
    });
    
	// events
    function placeItem(item, quantity, hp, maxhp, div, div2, div3, div4) {       
        $(div).css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/items/' + item + '.png)');
        if (isNotWep(item)) {
            if (quantity != '0') {
                $(div2).text(quantity);
                $(div2).show();
            }
            $(div3).hide();
        } else {
            $(div4).animate({height: ((hp / maxhp) * 100) + '%'}, 200);
            $(div3).show();
            if (quantity != '0')
                $(div2).hide();
        }
    }
    function isNotWep(item) {
        if (item != "bat" && !/stun/.test(item) &&
                item != "axe" && item != "sword" &&
                item != "kevlar" && item != "battle_axe" &&
                item != "chain_stick" && item != "crowbar" &&
                item != "fishing_rod" && item != "iron_bat" &&
                item != "lightsaber" && item != "long_sword" &&
                item != "metal_pipe" && item != "power_axe" &&
                item != "spike_ball" && item != "kevlar2" &&
                item != "kevlar3" && item != "kevlar4" &&
                item != "gold_bat" && item != "gold_battleaxe" &&
                item != "gold_chainstick" && item != "gold_crowbar" &&
                item != "gold_lightsaber" && item != "gold_pipe" &&
                item != "gold_poweraxe" && item != "gold_spikeball" && 
                item != "gold_longsword" && item != "skateboard" && item != "knife" &&
			    item != "candycane")
            return true;
        else return false;
    }
	function PetPosition(pet) {
        if (pet == 60){
		$("#pet").css({'left': '50px'});
		$("#pet").css({'top': '79px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/dog.png)');
		}
		else if (pet == 1){
		$("#pet").css({'left': '50px'});
		$("#pet").css({'top': '72px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/cat.png)');
		}
		else if (pet == 2){
		$("#pet").css({'left': '35px'});
		$("#pet").css({'top': '89px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/croc.png)');
		}
		else if (pet == 3){
		$("#pet").css({'left': '45px'});
		$("#pet").css({'top': '79px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/terrier.png)');
		}
		else if (pet == 4){
		$("#pet").css({'left': '35px'});
		$("#pet").css({'top': '79px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/bear.png)');
		}
		else if (pet == 5){
		$("#pet").css({'left': '42px'});
		$("#pet").css({'top': '79px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/pig.png)');
		}
		else if (pet == 6){
		$("#pet").css({'left': '14px'});
		$("#pet").css({'top': '79px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/lion.png)');
		}
		else if (pet == 7){
		$("#pet").css({'left': '14px'});
		$("#pet").css({'top': '79px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/rhino.png)');
		}
		else if (pet == 8){
		$("#pet").css({'left': '55px'});
		$("#pet").css({'top': '92px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/spider.png)');
		}
		else if (pet == 9){
		$("#pet").css({'left': '72px'});
		$("#pet").css({'top': '95px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/turtle.png)');
		}
		else if (pet == 10){
		$("#pet").css({'left': '62px'});
		$("#pet").css({'top': '90px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/chick.png)');
		}
		else if (pet == 11){
		$("#pet").css({'left': '62px'});
		$("#pet").css({'top': '97px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/frog.png)');
		}
		else if (pet == 12){
		$("#pet").css({'left': '52px'});
		$("#pet").css({'top': '71px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/dragon.png)');
		}
		else if (pet == 14){
		$("#pet").css({'left': '75px'});
		$("#pet").css({'top': '71px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/monkey.png)');
		}
		else if (pet == 15){
		$("#pet").css({'left': '60px'});
		$("#pet").css({'top': '81px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/horse.png)');
		}
	    else if (pet == 30){
		$("#pet").css({'left': '70px'});
		$("#pet").css({'top': '96px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/piglet.png)');
		}	
		else if (pet == 28){
		$("#pet").css({'left': '70px'});
		$("#pet").css({'top': '87px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/kitten.png)');
		}	
		else if (pet == 29){
		$("#pet").css({'left': '70px'});
		$("#pet").css({'top': '94px'});
		$("#pet").css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/profile/pet/puppy.png)');
		}	
    }
    function GlobalColor() {
        /*$("#stat_border").css("background", "-webkit-linear-gradient(top," + color1 + " 0," + color2 + " 100%)");*/
       
    }
    function startTimer(duration, display, sec) {
        var minutes = duration;
        var seconds = sec;
        var time = setInterval(function() {
            if (seconds == 0 && minutes > 0) {
                minutes -= 1;
                seconds = 59;
            } else if (seconds > 0)
                seconds -= 1;
            min = minutes < 10 ? "" + minutes : minutes;
            secs = seconds < 10 ? "0" + seconds : seconds;
            display.text("Time left: " + min + ":" + secs);
            if (timertype == 'jail')
                ws.send('Eventjailtimer.Name' + username + '.Data' + minutes + '.ExtraData' + seconds + '.Token' + token);
            else if (timertype == 'job')
                ws.send('Eventjobtimer.Name' + username + '.Data' + minutes + '.ExtraData' + seconds + '.Token' + token);
            if (timertype == 'stopwork') {
                clearInterval(time);
                istimer = false;
                $('#timer').hide();
            }
            if (minutes <= 0 && seconds <= 0) {
                if (timertype == 'jail') {
                    clearInterval(time);
                    istimer = false;
                    $('#timer').hide();
                } else if (timertype == 'job') {
                    minutes = 10;
                    seconds = 0;
                }

            }
            if (curtime > 0 || cursec > 0) {
                minutes = curtime;
                seconds = cursec;
                curtime = 0;
                cursec = 0;
            }
        }, 1000);
    }
    function CopAlert() {
        var i = 0;
        var time = setInterval(function() {
			if (copblink > 0) {
            if (i == 0) {
                $('#workbadge').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/cop/911one.png)');
                i = 1;
            } else {
                $('#workbadge').css('content', 'url(' + RPurl + '/app/tpl/skins/Habbo/ws/cop/911two.png)');
                i = 0;
            }           
            }


        }, 600);
    }
	function mouseMov(e) {
	if (/stun/.test(ItemMove) || ItemMove == 'fish' || ItemMove == 'skateboard')
	$('#itemdrag').css({left: e.pageX - 15, top: e.pageY - 15});
    else $('#itemdrag').css({left: e.pageX - 15, top: e.pageY - 20});
	if (prevItem != ''){
		if (ItemType == 'slot'){
		$('#slot'+ prevItem +'').css('content', 'url()');
		$('#whp'+prevItem+'').hide();
		$('#quantity'+prevItem+'').hide();
		}
		else {
		$('#bslot'+ prevItem +'').css('content', 'url()');
		$('#bhp'+prevItem+'').hide();
		$('#bquantity'+prevItem+'').hide();
		}
		prevItem = '';
		$('#itemdrag').show();	
	}	
    }
	function highlight(id){
   document.getElementById("wep_circle" +id+ "").style.boxShadow = "0 0 2px 2px #5fafb9";
   $('#wep_circle'+ id  +'').css('border', '2px solid #5fafb9');
   setTimeout(function(){
   document.getElementById("wep_circle" +id+ "").style.boxShadow = "0 0 2px 2px #45929c";
   $('#wep_circle' + id + '').css('border', '2px solid #45929c');
   }, 1000);
}

    connect();
}