<?php
/*==============================================================================
  Troposphir - Part of the Troposphir Project
  Copyright (C) 2016  Troposphir Development Team

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
if (!defined("INCLUDE_SCRIPT")) return;
class get_file extends RequestResponse {
	public function work($json) {
    if(!isset($json['project'])) return;
    if(!isset($json['body']['version']) || !isset($json['body']['remote_path'])) return;
    if(!isset($json['body']['version']['Major']) || !isset($json['body']['version']['Minor']) || !isset($json['body']['version']['Build'])) return;

    $remote_path = $json['body']['remote_path'];
    $project = $json['project'];
    $version = versionToString($json['body']['version']['Major'], $json['body']['version']['Minor'], $json['body']['version']['Build']);
    if(!projectExists($project, $this->config) || !versionExists($version, $json['project'], $this->config)) {
      $this->error('NOT_FOUND');
      return;
    }

    $path_before_version = getPathToProject($project, $this->config).DIRECTORY_SEPARATOR;
    $path_after_version = DIRECTORY_SEPARATOR.$remote_path;
    $file_path = $path_before_version.$version.$path_after_version;
    if(file_exists($file_path)) {
      $this->setResponseAsFile();
      $this->setContent(file_get_contents($file_path));
      return;
    } else {
      $lowerVersions = array_reverse(versionsLower($version, $project, $this->config));

      foreach($lowerVersions as $v) {
        $newPath = $path_before_version.$v.$path_after_version;
        if(file_exists($newPath)) {
          $this->setResponseAsFile();
          $this->setContent(file_get_contents($newPath));
          return;
        }
      }

      $this->error('NOT_FOUND');
      $this->addBody("path",$file_path);
      $this->addBody("rel",$remote_path);
    }
	}
}
?>
