<?php

namespace Revolution;
if(!defined('IN_INDEX')) { die('Sorry, you cannot access this file.'); }
class users implements iUsers
{
	
	/*-------------------------------Authenticate-------------------------------------*/ 
	
	final public function isLogged()
	{
		if(isset($_SESSION['user']['id']))
		{
			return true;
		}
		
		return false;
	}
	
	/*-------------------------------Checking of submitted data-------------------------------------*/ 
	
	final public function validName($username) 	
	{
		if(strlen($username) <= 25 && strlen($username) > 2 && ctype_alpha($username)) 		
	 	{ 			
	 		return true; 		
	 	} 		 		
	 	
	 	return false; 	
	} 	 	
		 
	final public function validEmail($email) 	
	{ 		
		return preg_match("/^[a-z0-9_\.-]+@([a-z0-9]+([\-]+[a-z0-9]+)*\.)+[a-z]{2,7}$/i", $email); 	
	} 	 	
	
	final public function validSecKey($seckey)
	{
		if(is_numeric($seckey) && strlen($seckey) == 4)
		{
			return true;
		}
		
		return false;
	}
	
	final public function nameTaken($username) 	
	{ 		
	 	global $engine; 		
	 	
		if($engine->num_rows("SELECT * FROM users WHERE username = '" . $username . "' LIMIT 1") > 0)
		{
			return true;
		} 	
		
		return false;
	} 
	
	final public function emailTaken($email)
	{
		global $engine;
		
		if($engine->num_rows("SELECT * FROM users WHERE mail = '" . $email . "' LIMIT 1") > 0)
		{
			return true;
		}
		
		return false;
	}
		
final public function userValidation($username, $password)
    {
        global $engine;
        if ($engine->num_rows("SELECT * FROM users WHERE username = '" . $username . "' AND password = '" . $password . "' LIMIT 1") > 0) {
            return true;
        }

        return false;
    }
	/*-------------------------------Stuff related to bans-------------------------------------*/ 
	
	final public function isBanned($value)
	{
		global $engine;
		if($engine->num_rows("SELECT * FROM bans WHERE value = '" . $value . "' LIMIT 1") > 0)
		{
			return true;
		}
			
		return false;
	}
	
	final public function getReason($value)
	{
		global $engine;
		return $engine->result("SELECT reason FROM bans WHERE value = '" . $value . "' LIMIT 1");
	}
	
	final public function hasClones($ip)
	{
		global $engine;
		if($engine->num_rows("SELECT * FROM users WHERE ip_reg = '" . $_SERVER['REMOTE_ADDR'] . "'") == 5)
		{
			return true;
		}
		
		return false;
	}
	
	/*-------------------------------Login or Register user-------------------------------------*/ 
	
	final public function register()
	{
		global $core, $template, $_CONFIG;
		
		if(isset($_POST['register']))
		{
			unset($template->form->error);
			//$template->form->error = 'Register is currently disabled.';
							//return;
			$template->form->setData();
				
			if($this->validName($template->form->reg_username))
			{
				if(!$this->nameTaken($template->form->reg_username))
				{
					if(strlen($template->form->reg_password) > 6)
					{
						if($template->form->reg_password == $template->form->reg_rep_password)
						{
							if($this->isBanned($_SERVER['REMOTE_ADDR']) == false)
							{
									if(!isset($template->form->reg_gender)) { $template->form->reg_gender = 'M'; }
									if($template->form->reg_gender == 'M') { $template->form->reg_figure = $_CONFIG['hotel']['malefigure']; }
								if($template->form->reg_gender == 'F') { $template->form->reg_figure = $_CONFIG['hotel']['femalefigure']; }
									$this->addUser($template->form->reg_username, $core->hashed($template->form->reg_password), $template->form->reg_email, $_CONFIG['hotel']['motto'], $_CONFIG['hotel']['credits'], $_CONFIG['hotel']['pixels'], 1, $template->form->reg_figure, $template->form->reg_gender, $core->hashed($template->form->reg_key));
					
									$this->turnOn($template->form->reg_username);
							
									header('Location: ' . $_CONFIG['hotel']['url'] . '/me');
									exit;
							}
							else
							{
								$template->form->error = 'Sorry, it appears you are IP banned.<br />';
								$template->form->error .= 'Reason: ' . $this->getReason($_SERVER['REMOTE_ADDR']);
								return;
							}
						}
						else	
						{
							$template->form->error = 'Password does not match repeated password';
							return;
						}

					}
					else
					{
						$template->form->error = 'Password must have more than 6 characters';
						return;
					}
				}
				else
				{
					$template->form->error = 'Username is already registered';
					return;
				}
			}
			else
			{
				$template->form->error = 'Username is invalid';
				return;
			}
		}
	}	
	
	final public function login()
	{
		global $template, $_CONFIG, $core;
		
		if(isset($_POST['login']))
		{
			$template->form->setData();
			unset($template->form->error);
			
			if($this->nameTaken($template->form->log_username))
			{
				if($this->isBanned($template->form->log_username) == false || $this->isBanned($_SERVER['REMOTE_ADDR']) == false)
				{
					if($this->userValidation($template->form->log_username, $core->hashed($template->form->log_password)))
					{
						$this->turnOn($template->form->log_username);
						$this->updateUser($_SESSION['user']['id'], 'ip_last', $_SERVER['REMOTE_ADDR']);
						$template->form->unsetData();
						header('Location: ' . $_CONFIG['hotel']['url'] . '/me');
						exit;
					}
					else
					{
						$template->form->error = 'Details do not match';
						return;
					}
				}
				else
				{
					$template->form->error = 'Sorry, it appears this user is banned<br />';
					$template->form->error .= 'Reason: ' . $this->getReason($template->form->log_username);
					return;
				}
			}
			else
			{
				$template->form->error = 'Username does not exist';
				return;
			}
		}
	}
	
	final public function loginHK()
	{
		global $template, $_CONFIG, $core;
		
		if(isset($_POST['login']))
		{	
			$template->form->setData();
			unset($template->form->error);
			
			if(isset($template->form->username) && isset($template->form->password))
			{
				if($this->nameTaken($template->form->username)) 
				{	 
					if($this->userValidation($template->form->username, $core->hashed($template->form->password)))
					{
						if(($this->getInfo($_SESSION['user']['id'], 'rank')) >= 7)
						{
							$_SESSION["in_hk"] = true;
							header("Location:".$_CONFIG['hotel']['url']."/stafflogin/index.php?url=dashboard");
							exit;
						}
						else
						{
							$template->form->error = 'Incorrect access level.';
							return;
						}					
						
					}
					else
					{
						$template->form->error = 'Incorrect password.';
						return;
					}		
				}
				else
				{
					$template->form->error = 'User does not exist.';
					return;
				}
			}
	
			$template->form->unsetData();
		}
	}	
	
	final public function help()
	{
		global $template, $_CONFIG;
		$template->form->setData();
		
		if(isset($template->form->help))
		{
			$to = $_CONFIG['hotel']['email'];
 			$subject = "Help from RevCMS user - " . $this->getInfo($_SESSION['user']['id'], 'username');
 			$body = $template->form->question;
 				
 			if (mail($to, $subject, $body))
 			{
 				$template->form->error = 'Message successfully sent! We will answer you shortly!';
 			} 
 			else 
 			{
  				 $template->form->error = 'Message delivery failed.';
 			}
		}
	}

	/*-------------------------------Account settings-------------------------------------*/ 
	
	final public function updateAccount()
	{
		global $template, $_CONFIG, $core, $engine;
		
		unset($template->form->error);
		
		if(isset($_POST['account']))
		{
			
			if(!empty($_POST['acc_old_password']) && !empty($_POST['acc_new_password']) && !empty($_POST['acc_new_password_again']))
			{
				if($this->userValidation($this->getInfo($_SESSION['user']['id'], 'username'), $core->hashed($_POST['acc_old_password'])))
				{
					if ($_POST['acc_new_password'] != $_POST['acc_new_password_again'])
					{
						$template->form->error = 'Passwords do not match.';
							return;
					}
					else
					{
						if(strlen($_POST['acc_new_password']) >= 8)
						{
							$this->updateUser($_SESSION['user']['id'], 'password', $core->hashed($_POST['acc_new_password']));
							$template->form->error = 'changed';
							return;
						}
						else
						{
							$template->form->error = 'New password is too short';
							return;
						}
					}
				}
				else
				{
					$template->form->error = 'Current password is wrong';
					return;
				}
			}
		}		
	}
		
		
	final public function turnOn($k)
	{	
		$j = $this->getID($k);
		$this->createSSO($j);
		$_SESSION['user']['id'] = $j;	
		$this->cacheUser($j);	
		unset($j);
	}
	
	/*-------------------------------Loggin forgotten-------------------------------------*/ 	
	
	final public function forgotten()
	{
		return;
	}
	
	/*-------------------------------Create SSO auth_ticket-------------------------------------*/ 
	
	final public function createSSO($k) 	
	{ 	 	
		$sessionKey = 'HabboRP-'.rand(9,999).'/'.substr(sha1(time()).'/'.rand(9,9999999).'/'.rand(9,9999999).'/'.rand(9,9999999),0,33);
		
		$webKey = 'WS'.uniqid();
		
		$this->updateUser($k, 'auth_ticket', $sessionKey);
		$this->updateUser($k, 'wstoken', $webKey);
		
		unset($sessionKey);
	} 	 
		
	/*-------------------------------Adding/Updating/Deleting users-------------------------------------*/ 
	final public function addUser($username, $password, $email, $motto, $credits, $pixels, $rank, $figure, $gender, $seckey) 	
	{ 		
		global $engine; 		 		 		 		
		$sessionKey = 'HabboRP-'.rand(9,999).'/'.substr(sha1(time()).'/'.rand(9,9999999).'/'.rand(9,9999999).'/'.rand(9,9999999),0,33);
        $engine->query("INSERT INTO users (username, password, mail, motto, credits, activity_points, rank, look, gender, seckey, ip_last, ip_reg, account_created, last_online, auth_ticket) VALUES('" . $username . "', '" . $password . "', '" . $email . "', '" . $motto . "', '" . $credits . "', '" . $pixels . "', '" . $rank . "', '" . $figure . "', '" . $gender . "', '" . $seckey . "', '" . $_SERVER['REMOTE_ADDR'] . "', '" . $_SERVER['REMOTE_ADDR'] . "', '" . time() . "', '" . time() . "', '" . $sessionKey . "')"); 	
		//$engine->query("INSERT INTO users_stats (name, id, home_room, health, hunger, energy, hits, strength, deaths, xp, xpdue, dead, jailed, roomx, roomy, kills, job, noob, arrests, jobrank, jobroom, rotation, loggedin, roomid, haspet, itemid) VALUES('" . $username . "', '0', '20', '100', '0', '100', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '600', '0', '0', '0', '0', '0', '0', '0', '0')"); 	
		unset($sessionKey);	
		 			 
	}	 		 	
		 
	final public function deleteUser($k) 	
	{ 		
		global $engine; 		 		
	 	$engine->query("DELETE FROM users WHERE id = '" . $k . "' LIMIT 1"); 		
	 	$engine->query("DELETE FROM items WHERE userid = '" . $k . "' LIMIT 1"); 		
		$engine->query("DELETE FROM rooms WHERE ownerid = '" . $k . "' LIMIT 1"); 	
	} 	
	  	
	final public function updateUser($k, $key, $value) 	
	{ 		
	 	global $engine; 		 		
	 	$engine->query("UPDATE users SET " . $key . " = '" . $engine->secure($value) . "' WHERE id = '" . $k . "' LIMIT 1");
	 	$_SESSION['user'][$key] = $engine->secure($value);		
	} 
	
	/*-------------------------------Handling user information-------------------------------------*/ 	 
	
	final public function cacheUser($k)
	{
		global $engine; 		 	
		$userInfo = $engine->fetch_assoc("SELECT username, rank, motto, mail, credits, activity_points, look, auth_ticket, ip_last FROM users WHERE id = '" . $k . "' LIMIT 1");
		
		foreach($userInfo as $key => $value)
		{
			$this->setInfo($key, $value);
		}
	}	
	
	final public function setInfo($key, $value)
	{
		global $engine;
		$_SESSION['user'][$key] = $engine->secure($value);
	}

	final public function getInfo($k, $key)
	{
		global $engine;
		if(!isset($_SESSION['user'][$key]))
		{
			$value = $engine->result("SELECT $key FROM users WHERE id = '" . $engine->secure($k) . "' LIMIT 1", $key); 
			if($value != null)
			{			
				$this->setInfo($key, $value);
			}
		}
			
		return $_SESSION['user'][$key];
	}
	
	
	
	/*-------------------------------Get user ID or Username-------------------------------------*/ 
	
	final public function getID($k) 	
	{ 		
		global $engine; 		
		return $engine->result("SELECT id FROM users WHERE username = '" . $engine->secure($k) . "' LIMIT 1", "id"); 	
	} 		
	
	final public function getUsername($k)
	{
		global $engine;
		return $this->getInfo($_SESSION['user']['id'], 'username');
	}
		final public function getStaff($rank)
    {
      global $engine;
      return $engine->fetch_array("SELECT username,motto,look,role,online FROM users WHERE rank = '".$rank."'");
    }
}
?>