<?php

namespace Revolution;
if(!defined('IN_INDEX')) { die('Sorry, you cannot access this file.'); }
class engine
{
	private $initiated;
	private $connected;
	
	private $connection;
	
final public function __construct()
	{
	global $core, $_CONFIG;
		if(!$this->connected)
		{
			$this->connection = new \MySQLi($_CONFIG['mysql']['hostname'],  $_CONFIG['mysql']['username'], $_CONFIG['mysql']['password'], $_CONFIG['mysql']['database']) or exit($core->systemError('MySQLI Engine', 'MySQL could not connect to host or database'));
			$this->connected = true;
			
		}
	}
	final public function Initiate()
	{
		global $_CONFIG;
		//nothing to see here
	}
	
	final public function setMySQL($key, $value)
	{
		//nothing to see here
	}
	
	
	
	/*-------------------------------Manage Connection-------------------------------------*/
	
	final public function connect($type)
	{
/*	global $core, $_CONFIG;
		if(!$this->connected)
		{
			$this->connection = new \MySQLi($_CONFIG['mysql']['hostname'],  $_CONFIG['mysql']['username'], $_CONFIG['mysql']['password'], $_CONFIG['mysql']['database']) or exit($core->systemError('MySQLI Engine', 'MySQL could not connect to host or database'));
			$this->connected = true;
			
		}*/
	}
	
	final public function disconnect()
	{
		global $core;
		if($this->connected)
		{
			if($this->connection->close())
			{
				$this->connected = false;
			}
			else
			{
				$core->systemError('MySQLI Engine', 'MySQLI could not disconnect.');
			}
		}
	}
	
	/*-------------------------------Secure MySQL variables-------------------------------------*/
	
	final public function secure($var)
	{
		return $this->connection->real_escape_string(stripslashes(htmlspecialchars($var)));
	}
	
	/*-------------------------------Manage MySQL queries-------------------------------------*/
	
	final public function query($sql)
	{
		return $this->connection->query($sql) or die($this->connection->error);
	}
	
	final public function num_rows($sql)
	{
		$numQuery = $this->connection->query($sql) or die($this->connection->error);
		return $numQuery->num_rows;
		
	}
	
	final public function result($sql, $colum)
	{
		$rQuery = $this->connection->query($sql) or die($this->connection->error);
		$rFetch = $rQuery->fetch_array();
		return $rFetch[$colum];
	}
	
/*	final public function free_result($sql)
	{
		return $this->mysql['free_result']($sql);
	}*/
	
	final public function fetch_array($sql)
	{
		$query = $this->connection->query($sql) or die("1");
		
		$data = array();
		
		while($row = $query->fetch_array())
		{
			$data[] = $row;
		}
		
		return $data;
	}
	
	final public function fetch_assoc($sql)
	{
		$fQuery = $this->connection->query($sql) or die('lol');
		return $fQuery->fetch_assoc();
	}




}
?>
