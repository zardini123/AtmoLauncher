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
class get_version extends RequestResponse {
	public function work($json) {

    if(!isset($json['project'])) return;
    if(!projectExists($json['project'], $this->config)) return;

    $versions = allVersions($json['project'], $this->config);
    if($versions != -1){
      $versions_desc = array_reverse($versions);
      $latest = $versions_desc[0];

      $versionParts = explode(".", $latest);

      $this->addBody("Latest", $latest);
      $this->addBody("Major", (int)$versionParts[0]);
      $this->addBody("Minor", (int)$versionParts[1]);
      $this->addBody("Build", (int)$versionParts[2]);
    } else {
      $this->error("NOT_FOUND");
    }
	}
}
?>
