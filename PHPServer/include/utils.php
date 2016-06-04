<?php
/*==============================================================================
	Troposphir - Part of the Troposphir Project
    Copyright (C) 2013  Troposphir Development Team

	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU Affero General Public License as
	published by the Free Software Foundation, either version 3 of the
	License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Affero General Public License for more details.

	You should have received a copy of the GNU Affero General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>.
==============================================================================*/

function startsWith($haystack, $needle) {
    return strpos($haystack, $needle) === 0;
}

function endsWith($haystack, $needle) {
    return strpos($haystack, $needle) + strlen($needle) === strlen($haystack);
}

function isVersion($input){
  $parts = explode(".", $input);

  if(count($parts) != 3) return false;

  foreach($parts as $part){
    if(!is_numeric($part)) return false;
  }

  return true;
}

function allVersions($project, $config){
  $projects_dir = "./" . $config['projects_root'];
  $project_name = $project;
  $project_dir = $projects_dir . "/" . $project_name;
  if(file_exists($project_dir)){
    $project_contents = array_diff(scandir($project_dir), array('..', '.'));
    $versions = array();
    foreach($project_contents as $file){
      if(isVersion($file)){
        $versions[] = $file;
      }
    }

    usort($versions, "version_compare");
    return $versions;
  } else return -1;
}

function allVersions_d($project, $config){
  return array_reverse(allVersions($project, $config));
}

function latestVersion($project, $config){
  return allVersions_d($project, $config)[0];
}

function versionToString($major, $minor, $build){
  return $major . "." . $minor . "." . $build;
}

function versionsHigher($version, $project, $config){
  $versions = allVersions($project, $config);

  $vret = array();
  foreach($versions as $v){
    if (version_compare($v, $version, '>')) {
      $vret[] = $v;
    }
  }

  return $vret;
}

function versionsLower($version, $project, $config){
  $versions = allVersions($project, $config);

  $vret = array();
  foreach($versions as $v){
    if (version_compare($v, $version, '<')) {
      $vret[] = $v;
    }
  }

  return $vret;
}

function versionsBetween($versionA, $versionB, $project, $config){
  $versions = allVersions($project, $config);

  $vret = array();
  foreach($versions as $v){
    if (version_compare($v, $versionA, '>') && version_compare($v, $versionB, '<=')) {
      $vret[] = $v;
    }
  }

  return $vret;
}

function projectExists($project, $config){
  $project_dir = getPathToProject($project, $config);
  if(file_exists($project_dir)) return true;

  return false;
}

function versionExists($version, $project, $config){
  $projects_dir = "./" . $config['projects_root'];
  $project_name = $project;
  $project_dir = $projects_dir . "/" . $project_name;
  if(file_exists($project_dir."/".$version)) return true;

  return false;
}

function getPathToProject($project, $config) {
  return "." . DIRECTORY_SEPARATOR . $config['projects_root'] . DIRECTORY_SEPARATOR . $project;
}

?>
