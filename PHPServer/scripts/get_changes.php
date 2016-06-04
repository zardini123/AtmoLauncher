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
class get_changes extends RequestResponse {
	public function work($json) {
    if(!isset($json['project'])) return;
    if(!isset($json['body']['current']) || !isset($json['body']['target'])) return;
    if(!isset($json['body']['current']['Major']) || !isset($json['body']['current']['Minor']) || !isset($json['body']['current']['Build'])) return;
    if(!isset($json['body']['target']['Major']) || !isset($json['body']['target']['Minor']) || !isset($json['body']['target']['Build'])) return;

    if(!projectExists($json['project'], $this->config)) return;

    $current = versionToString($json['body']['current']['Major'], $json['body']['current']['Minor'], $json['body']['current']['Build']);
    $target = versionToString($json['body']['target']['Major'], $json['body']['target']['Minor'], $json['body']['target']['Build']);

    $projects_dir = "./" . $this->config['projects_root'];
    $project_name = $json['project'];
    $project_dir = $projects_dir . DIRECTORY_SEPARATOR . $project_name;

    $versionDifference = array_reverse(versionsBetween($current, $target, $project_name, $this->config));

    $changed_files = array();
    foreach($versionDifference as $vd){
      foreach (new RecursiveIteratorIterator(new RecursiveDirectoryIterator($project_dir . DIRECTORY_SEPARATOR . $vd)) as $filename)
      {
        // filter out "." and ".."
        if ($filename->isDir()) continue;

        $file = str_replace($project_dir . DIRECTORY_SEPARATOR . $vd, "", $filename->getPath());
        $file = $file . DIRECTORY_SEPARATOR . $filename->getFileName();

        if(startsWith($file, DIRECTORY_SEPARATOR)){
          $file = substr($file, 1);
        }

        if(!array_key_exists($file, $changed_files)){
          $changed_files[$file] = $filename->getSize();
        }
      }
    }

    $this->addBody("new_sizes", $changed_files);
	}
}
?>
