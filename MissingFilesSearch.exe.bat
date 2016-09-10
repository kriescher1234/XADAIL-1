SET BATCH_DIR=%~dp0

rem sample: 
rem MissingFilesSearch.exe /path:"E:\Working\Grim Dawn\mods\XADAIL-1\DAIL\database" /GrimDawnPath:"E:\Working\Grim Dawn\database"

rem /path path to check
rem /GrimDawnPath path to extracted GD extracted game files (should not contain extracted DAIL stuff)
rem /IgnorePattern ignore files that contain any of these patterns 
rem /WhitePattern:"storm" filters files from /path parameter (if you are looking for a special folder for example)
MissingFilesSearch.exe /path:"%BATCH_DIR%DAIL\database" /GrimDawnPath:"E:\SteamLibrary\steamapps\common\Grim Dawn" /IgnorePattern:"\sounds\;petskill_abomination_scaling;Class29;passive_totaldamageabsorption01;perlevelx100;defense_undeadresists;resist_undead;melee_lifeleech_01;armor_passive;trap_resists;loottables;lootaffixes;m002_mutantarml01;m002_mutantarmr01" /WhitePattern:"skills"
rem MissingFilesSearch.exe /path:"%BATCH_DIR%DAIL - Survival\database" /GrimDawnPath:"E:\Working\Grim Dawn\database" /IgnorePattern:"\sounds\;\loottables\\"