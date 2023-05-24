create database `s3cr3tx`;

USE `s3cr3tx`;
 

CREATE TABLE `Auth`(
	`vID` bigint AUTO_INCREMENT NOT NULL,
	`vEmail` nvarchar(256) NOT NULL,
	`vIsValid` Tinyint NOT NULL DEFAULT 0,
	`CreateDate` datetime(3) NOT NULL DEFAULT utc_timestamp(3),
	`UpdateDate` datetime(3) NOT NULL DEFAULT utc_timestamp(3),
	`CreatedBy` nvarchar(256) NOT NULL,
	`UpdatedBy` nvarchar(256) NOT NULL,
	`AuthCode` nvarchar(50) NOT NULL,
 CONSTRAINT `PK_Auth` PRIMARY KEY 
(
	`vID` ASC
) 
);

CREATE TABLE `files`(
	`fileID` bigint AUTO_INCREMENT NOT NULL,
	`fileOwnerEmail` nvarchar(256) NOT NULL,
	`fileOriginalName` nvarchar(256) NOT NULL,
	`fileDirPath` nvarchar(512) NOT NULL,
	`fileDbPath` nvarchar(512) NOT NULL,
	`fileVirtualPath` nvarchar(512) NULL,
	`fileDescription` nvarchar(512) NULL,
	`CreateDate` datetime(3) NOT NULL DEFAULT utc_timestamp(3),
 CONSTRAINT `PK_files` PRIMARY KEY 
(
	`fileID` ASC
) 
);

CREATE TABLE `forgot`(
	`reset_id` bigint AUTO_INCREMENT PRIMARY KEY NOT NULL,
	`reset_email` nvarchar(256) NOT NULL,
	`reset_code` nvarchar(50) NOT NULL,
	`reset_start` datetime(3) NOT NULL,
	`reset_expires` datetime(3) NOT NULL,
	`req_IP` nvarchar(256) NOT NULL,
	`member_id` bigint NOT NULL
);

CREATE TABLE `m`(
	`mID` bigint AUTO_INCREMENT NOT NULL,
	`mGuid1` nvarchar(50) NOT NULL,
	`mGuid2` nvarchar(50) NOT NULL,
	`mEmail` nvarchar(256) NOT NULL,
	`mAPIKey` nvarchar(256) NOT NULL,
	`mAuthorization` nvarchar(512) NOT NULL,
	`mPrivateKey` Longtext NOT NULL,
	`mPublicKey` Longtext NOT NULL,
	`mCreateTime` datetime(3) NOT NULL DEFAULT utc_timestamp(3),
	`mPmHash` nvarchar(256) NULL,
	`mUpdateTime` datetime(3) NULL,
 CONSTRAINT `PK_m` PRIMARY KEY 
(
	`mID` ASC
) 
) ;

CREATE TABLE `member_archive`(
	`member_archive_id` bigint AUTO_INCREMENT NOT NULL,
	`member_id` bigint NOT NULL,
	`member_code` nvarchar(512) NOT NULL,
	`current_code` Tinyint NOT NULL,
	`create_date` datetime(3) NOT NULL,
	`update_date` datetime(3) NOT NULL,
 CONSTRAINT `PK_member_archive` PRIMARY KEY 
(
	`member_archive_id` ASC
) 
);

CREATE TABLE `member_sessions`(
	`session_id` bigint AUTO_INCREMENT NOT NULL,
	`member_id` bigint NOT NULL,
	`member_email` nvarchar(256) NOT NULL,
	`member_token` nvarchar(50) NOT NULL,
	`member_name` nvarchar(50) NOT NULL,
	`session_start` datetime(3) NOT NULL,
	`session_active` Tinyint NOT NULL,
	`session_last_active` datetime(3) NOT NULL,
	`session_end` datetime(3) NOT NULL,
	`member_ip` nvarchar(256) NOT NULL,
 CONSTRAINT `PK_NewTable` PRIMARY KEY 
(
	`session_id` ASC
) 
);

CREATE TABLE `members`(
	`member_id` bigint AUTO_INCREMENT NOT NULL,
	`member_email` nvarchar(256) NOT NULL,
	`member_code` Longtext NOT NULL,
	`member_first_name` nvarchar(25) NOT NULL,
	`member_last_name` nvarchar(25) NOT NULL,
	`member_reg_number` nvarchar(40) NOT NULL,
	`member_create_date` datetime(3) NOT NULL,
	`member_update_date` datetime(3) NOT NULL,
	`member_enabled` Tinyint NOT NULL DEFAULT 0,
	`member_confirmed` Tinyint NOT NULL DEFAULT 0,
	`member_country` nvarchar(30) NULL,
	`member_state` nvarchar(25) NULL,
	`member_gender` nvarchar(20) NULL,
	`member_mobile_phone` nvarchar(20) NULL,
	`member_mobile_carrier` nvarchar(25) NULL,
	`member_city` nvarchar(30) NULL,
	`member_zip` nvarchar(10) NULL,
	`member_address` nvarchar(50) NULL,
	`member_address2` nvarchar(50) NULL,
	`member_order_number` nvarchar(50) NULL DEFAULT 0,
 CONSTRAINT `PK_members` PRIMARY KEY 
(
	`member_id` ASC
) 
) ;

CREATE TABLE `msg`(
	`msg_ID` bigint AUTO_INCREMENT NOT NULL,
	`msg_from` nvarchar(256) NULL,
	`msg_to` nvarchar(256) NULL,
	`msg_text` Longtext NULL,
	`msg_attachments` Tinyint NULL DEFAULT 0,
	`msg_sent` datetime(3) NULL,
	`msg_delivered` datetime(3) NOT NULL,
	`msg_received` datetime(3) NOT NULL,
	`msg_deleted` Tinyint NULL DEFAULT 0,
	`msg_DeletedTime` datetime(3) NULL,
 CONSTRAINT `PK_msg` PRIMARY KEY 
(
	`msg_ID` ASC
) 
) ;

CREATE TABLE `msg_attachments`(
	`fileID` bigint AUTO_INCREMENT NOT NULL,
	`msg_id` bigint NOT NULL,
	`fileOwnerEmail` nvarchar(256) NOT NULL,
	`fileOriginalName` nvarchar(256) NOT NULL,
	`fileDirPath` nvarchar(512) NOT NULL,
	`fileDbPath` nvarchar(512) NOT NULL,
	`fileVirtualPath` nvarchar(512) NULL,
	`fileDescription` nvarchar(512) NULL,
	`CreateDate` datetime(3) NOT NULL DEFAULT utc_timestamp(3),
	`isDeleted` Tinyint NOT NULL DEFAULT 0,
	`DeletedDate` datetime(3) NULL,
 CONSTRAINT `PK_msg_attachments` PRIMARY KEY 
(
	`fileID` ASC
) 
);

CREATE TABLE `tblLog`(
	`LogID` bigint AUTO_INCREMENT NOT NULL,
	`LogSource` nvarchar(50) NOT NULL,
	`LogMessage` Longtext NOT NULL,
	`LogCreateDate` datetime(3) NOT NULL DEFAULT utc_timestamp(3),
 CONSTRAINT `PK_tblLog` PRIMARY KEY 
(
	`LogID` ASC
) 
);

CREATE procedure `checkAuth`
(p_APIKey nvarchar(256),
p_Authorization nvarchar(512),
p_email nvarchar(256))
begin
declare v_result int; 
set v_result = (Select count(*) from Auth where vIsValid = 1 and vEmail = p_email);
if v_result > 0
then
Select count(*) as `cnt` from m where mEmail = p_email and mAPIKey = p_APIKey and mAuthorization = p_Authorization ;
else
Select 0 as `cnt`;
End if;
END;
 

CREATE procedure `checkCredentials`
(p_email nvarchar(256),
p_pword nvarchar(512)
)
begin
declare v_result int; 
set v_result = (Select count(*) from Auth members where  vIsValid = 1 and vEmail = p_email);
if v_result = 1
then
Select count(*) as `cnt` from members where member_email = p_email and member_code = p_pword  and member_enabled = 1 and member_confirmed = 1;
else
Select 0 as `cnt`;
End if;
END;
 
Create procedure `EorD` (
p_Auth nvarchar(512),
p_APIToken nvarchar(256),
 p_email longtext,
 p_input longtext,
 p_EorD tinyint,
 p_blnDefault tinyint,
 out p_strOutput longtext)
 begin
Declare v_isAuth int;
declare v_strText longtext;
Set v_isAuth = (Select count(*) from m where mEmail = p_email and mAuthorization = p_Auth and mAPIKey = p_APIToken);
if v_isAuth > 0 
Then
set p_strOutput = '+True';
else
set p_strOutput = '';
End if;
END;
 

  

CREATE PROCEDURE `getBundle` ( 
	p_email nvarchar(256))

BEGIN
	SELECT `mID`, `mGuid1`, `mGuid2`, `mEmail`, `mAPIKey`, `mAuthorization`, `mPrivateKey`, `mPublicKey`
	from m
	where mEmail = p_email; --  SQLINES DEMO *** IKey and mAuthorization = @mAuthorization
END;
 



CREATE procedure `GetCode`
(p_email nvarchar(256)
)
begin
declare v_result int; 
set v_result = (Select count(*) from members where member_confirmed = 1 and member_email = p_email);
if v_result = 1
then
Select member_code from members where member_email = p_email and p_member_confirmed = 1;
else
Select '';
End if;
END;

create procedure `insertAuth`(
p_Email nvarchar(256),
p_IsValid tinyint,
p_CreatedBY nvarchar(256),
p_AuthCode nvarchar(50)
)
begin 
declare v_timestamp datetime(3);
set v_timestamp = (Select UTC_TIMESTAMP(3));

INSERT INTO `Auth`
           (`vEmail`
           ,`vIsValid`
           ,`CreateDate`
           ,`UpdateDate`
           ,`CreatedBy`
           ,`UpdatedBy`
           ,`AuthCode`)
     VALUES
           (p_Email
           ,p_IsValid
           ,v_timestamp
           ,v_timestamp
           ,p_CreatedBY
           ,p_CreatedBY
           ,p_AuthCode);
END;
 


  

create procedure `insertFile`(
p_fileOwner nvarchar(256),
           p_fileOrigName nvarchar(256)
           ,p_fileDirPath nvarchar(512)
           ,p_fileDbPath nvarchar(512)
           ,p_fileVirtualPath nvarchar(512) /* = null */
           ,p_fileDescription nvarchar(512) /* = null */
           )
 begin
		   declare v_CreateDate datetime(3);
		   set v_CreateDate = (Select UTC_TIMESTAMP(3));

INSERT INTO `files`
           (`fileOwnerEmail`
           ,`fileOriginalName`
           ,`fileDirPath`
           ,`fileDbPath`
           ,`fileVirtualPath`
           ,`fileDescription`
           ,`CreateDate`)
     VALUES
           (p_fileOwner,
           p_fileOrigName
           ,p_fileDirPath
           ,p_fileDbPath
           ,p_fileVirtualPath
           ,p_fileDescription
		   ,v_CreateDate);
END;
 
CREATE procedure `insertLog` (
p_Source nvarchar(50),
p_logMessage longtext)
begin 

INSERT INTO `tblLog`
           (`LogSource`
           ,`LogMessage`
           ,`LogCreateDate`)
     VALUES
           (p_Source
           ,p_logMessage
           ,UTC_TIMESTAMP(3));
END;
 
  

CREATE procedure `InsertM`(
p_Guid1 nvarchar(50),
p_Guid2 nvarchar(50),
p_Email nvarchar(256),
p_APIKey nvarchar(256),
p_Authorization nvarchar(512),
p_PmHash nvarchar(256),
p_PrivateKey longtext,
p_PublicKey longtext
)
sp_lbl:
begin
declare v_valid tinyint; declare v_TimeStamp datetime(3);
set v_TimeStamp = (Select UTC_TIMESTAMP(3));
set v_valid = (Select vIsValid from Auth where vEmail = p_Email);
if v_valid = 1
Then


INSERT INTO `m`
           (`mGuid1`
           ,`mGuid2`
           ,`mEmail`
           ,`mAPIKey`
           ,`mAuthorization`
           ,`mPrivateKey`
           ,`mPublicKey`
           ,`mCreateTime`
		   ,`mPmHash`
		   ,`mUpdateTime`)
     VALUES
           (p_Guid1
           ,p_Guid2
           ,p_Email
           ,p_APIKey
           ,p_Authorization
           ,p_PrivateKey
           ,p_PublicKey
           ,v_TimeStamp
		   ,p_PmHash
		   ,v_TimeStamp);
		   leave sp_lbl 1;
else
leave sp_lbl 0;
end if;
END;
 

  

CREATE PROCEDURE `isValidEmail` 
	(p_email nvarchar(256)) 
Begin
	
	
	SELECT Count(*) from Auth where vEmail = p_email and vIsValid = 1;
End;
  

CREATE PROCEDURE `USP_GetBundle_M` ( 
	p_mEmail nvarchar(256),
	p_mAPIKey nvarchar(256),
	p_mAuthorization nvarchar(512))
BEGIN
	SELECT `mID`, `mGuid1`, `mGuid2`, `mEmail`, `mAPIKey`, `mAuthorization`, `mPrivateKey`, `mPublicKey`
	from m
	where mEmail = p_mEmail and mAPIKey = p_mAPIKey and mAuthorization = p_mAuthorization;
END;
 

CREATE PROCEDURE `USP_GetBundle_M_Host` ( 
	p_mEmail nvarchar(256),
    p_mHost nvarchar(256))
BEGIN
	SELECT `mID`, `mGuid1`, `mGuid2`, `mEmail`, `mAPIKey`, `mAuthorization`, `mPrivateKey`, `mPublicKey`
	from m
	where mEmail = p_mEmail and  p_mHost = 's3cr3tx.com';
END;

  

CREATE PROCEDURE `usp_tbl_forgot_ins`
  (p_member_email nvarchar(256),p_member_token nvarchar(50),p_session_start datetime(3),p_session_end datetime(3), p_member_ip nvarchar(256),p_member_id bigint)
  BEGIN
       
       INSERT INTO `forgot`
        (`reset_email`,`reset_code`,`reset_start`,`reset_expires`,`req_IP`,`member_id`)
      VALUES
        (p_member_email,p_member_token,p_session_start,p_session_end,p_member_ip, p_member_id);
    END;
 

CREATE PROCEDURE `usp_tbl_forgot_sel`
  (p_member_email nvarchar(256),p_member_token nvarchar(50))
BEGIN
DECLARE v_default DATETIME(3);
BEGIN
 

SELECT member_id, reset_expires
      FROM `s3cr3tx`.`forgot` where reset_email = p_member_email and reset_code = p_member_token;
  END;

END; 

CREATE PROCEDURE `usp_tbl_member_ins`
  (p_member_email nvarchar(256),p_member_code longtext,p_member_first_name nvarchar(25),p_member_last_name nvarchar(25),p_member_reg_number nvarchar(40),p_member_create_date datetime(3),p_member_update_date datetime(3),p_ID bigint/* =NULL */,p_member_country nvarchar(30)/* =NULL */,p_member_state nvarchar(25)/* =NULL */,p_member_gender nvarchar(20)/* =NULL */,p_member_mobile_phone nvarchar(20)/* =NULL */,p_member_mobile_carrier nvarchar(25)/* =NULL */,p_member_city nvarchar(30)/* =NULL */,p_member_zip nvarchar(10)/* =NULL */,p_member_address nvarchar(50)/* =NULL */,p_member_address2 nvarchar(50)/* =NULL */,p_member_order nvarchar(50)/* =NULL */)
BEGIN
Declare v_result bigint;
set v_result = 0;
BEGIN
  IF (Select COUNT(*) from members where member_email = p_member_email and member_enabled = 1 and member_confirmed = 1) = 0 
    THEN
      
      INSERT INTO `members`
        (`member_email`,`member_code`,`member_first_name`,`member_last_name`,`member_reg_number`,`member_create_date`,`member_update_date`,`member_country`,`member_state`,`member_gender`,`member_mobile_phone`,`member_mobile_carrier`,`member_city`,`member_zip`,`member_address`,`member_address2`,`member_order_number`)
      VALUES
       (p_member_email,p_member_code,p_member_first_name,p_member_last_name,p_member_reg_number,p_member_create_date,p_member_update_date,p_member_country,p_member_state,p_member_gender,p_member_mobile_phone,p_member_mobile_carrier,p_member_city,p_member_zip,p_member_address,p_member_address2,p_member_order);
      set v_result = (SELECT member_id FROM `members` WHERE `member_id` = LAST_INSERT_ID());
      
      Select v_result;
  ELSE
      set v_result = (Select 0);
      
      select v_result;
    END IF;
END;

END;


CREATE PROCEDURE `usp_tbl_member_login_sel`
  (p_email nvarchar(256))
BEGIN
  IF p_email IS NULL OR p_email = '' THEN
    
    SELECT '';
  ELSE
    
    SELECT member_reg_number FROM `members` WHERE `member_email` = p_email and member_enabled = 1 and member_confirmed = 1;
  END IF;
END;
 

CREATE PROCEDURE `usp_tbl_member_sel`
  (p_member_id bigint/* =NULL */)
BEGIN
  IF p_member_id IS NULL OR p_member_id = 0 THEN
    
    SELECT * FROM `members` ORDER BY `member_id` ASC;
  ELSE
    
    SELECT * FROM `members` WHERE `member_id` = p_member_id;
  END IF;
END;

CREATE PROCEDURE `usp_tbl_member_sel_email`
  (p_email nvarchar(256))
BEGIN
    
    SELECT * FROM `members` WHERE `member_email` = p_email and member_confirmed = 1;
END;

CREATE PROCEDURE `usp_tbl_member_sel_reg`
  (p_email nvarchar(256))
BEGIN
    
    SELECT member_reg_number FROM `members` WHERE `member_email` = p_email and member_confirmed = 1;
End;

CREATE PROCEDURE `usp_tbl_member_sel_reg_confirm`
  (p_email nvarchar(256),p_reg_number nvarchar(256))
BEGIN
Declare v_emptyResult bigint;
set v_emptyResult = 0;
if (SELECT count(*) FROM `members` WHERE `member_email` = p_email and member_confirmed = 0 and member_reg_number = p_reg_number) =1
THEN
    
    SELECT member_id FROM `members` WHERE `member_email` = p_email and member_confirmed = 0 and member_reg_number = p_reg_number;
ELSE

select v_emptyResult;
END IF;

END;
   

CREATE PROCEDURE `usp_tbl_member_sessions_ins`
  (p_member_id bigint,p_member_email nvarchar(256),p_member_token nvarchar(50),p_member_name nvarchar(50),p_session_start datetime(3),p_session_active tinyint,p_session_last_active datetime(3),p_session_end datetime(3), p_member_ip nvarchar(256))
BEGIN
  IF p_member_id IS NULL OR p_member_id = 0
    THEN
      
      SELECT 0;
  ELSE
       
       INSERT INTO `member_sessions`
        (`member_id`,`member_email`,`member_token`,`member_name`,`session_start`,`session_active`,`session_last_active`,`session_end`,`member_ip`)
      VALUES
        (p_member_id,p_member_email,p_member_token,p_member_name,p_session_start,p_session_active,p_session_last_active,p_session_end,p_member_ip);
      
      SELECT * FROM `member_sessions` WHERE `session_id` = LAST_INSERT_ID();
    END IF;
END;

  

CREATE PROCEDURE `usp_tbl_member_sessions_sel`
  (p_ID bigint)
BEGIN
  IF p_ID IS NULL OR p_ID = 0 THEN
    
    SELECT * FROM `member_sessions` ORDER BY `session_id` ASC;
  ELSE
    
    SELECT * FROM `member_sessions` WHERE `session_id` = p_ID;
  END IF;
END;
 

CREATE PROCEDURE `usp_tbl_member_sessions_selCode`
  (p_ID nvarchar(50))
BEGIN
  IF p_ID IS NOT NULL OR p_ID != '' THEN
    
    SELECT * FROM `member_sessions` WHERE `member_token` = p_ID;
  END IF;
END;
  

CREATE PROCEDURE `usp_tbl_member_sessions_ups`
  (p_member_id bigint,p_member_email nvarchar(256),p_member_token nvarchar(50),p_member_name nvarchar(50),p_session_start datetime(3),p_session_active tinyint,p_session_last_active datetime(3),p_session_end datetime(3),p_member_ip nvarchar(256),p_ID bigint/* =NULL */)
BEGIN
  IF p_ID IS NULL OR p_ID = 0
    THEN
      
      INSERT INTO `member_sessions`
        (`member_id`,`member_email`,`member_token`,`member_name`,`session_start`,`session_active`,`session_last_active`,`session_end`,`member_ip`)
      VALUES
        (p_member_id,p_member_email,p_member_token,p_member_name,p_session_start,p_session_active,p_session_last_active,p_session_end,p_member_ip);
      
      SELECT * FROM `member_sessions` WHERE `session_id` = LAST_INSERT_ID();
  ELSE
      UPDATE `member_sessions`
        SET `member_id`=p_member_id,`member_email`=p_member_email,`member_token`=p_member_token,`member_name`=p_member_name,`session_start`=p_session_start,`session_active`=p_session_active,`session_last_active`=p_session_last_active,`session_end`=p_session_end, `member_ip` = p_member_ip
        WHERE (`session_id` = p_ID);
      
      SELECT * FROM `member_sessions` WHERE `session_id` = p_ID;
    END IF;
END;


CREATE PROCEDURE `usp_tbl_member_set_confirmed`
  (p_email nvarchar(256), p_reg_number nvarchar(40))
BEGIN
    Update members set member_confirmed = 1, member_enabled = 1 WHERE `member_email` = p_email and `member_reg_number` = p_reg_number;
END;


CREATE PROCEDURE `usp_tbl_member_set_newp`
  (p_email nvarchar(256), p_member_code longtext,p_member_id bigint)
BEGIN
    Update members set member_code = p_member_code WHERE `member_email` = p_email and `member_id` = p_member_id;
END;
  

CREATE PROCEDURE `usp_tbl_member_update`
  (p_member_id bigint,p_member_email nvarchar(256),p_member_first_name nvarchar(25),p_member_last_name nvarchar(25),p_member_reg_number nvarchar(30),p_member_create_date datetime(3),p_member_update_date datetime(3),p_member_country nvarchar(30)/* =NULL */,p_member_state nvarchar(25)/* =NULL */,p_member_gender nvarchar(20)/* =NULL */,p_member_mobile_phone nvarchar(20)/* =NULL */,p_member_mobile_carrier nvarchar(25)/* =NULL */,p_member_city nvarchar(30)/* =NULL */,p_member_zip nvarchar(10)/* =NULL */,p_member_address nvarchar(50)/* =NULL */,p_member_address2 nvarchar(50)/* =NULL */,p_member_order nvarchar(50)/* =NULL */)
BEGIN
  IF (Select COUNT(*) from members where `member_id` = p_member_id and member_email = p_member_email and member_enabled = 1 and member_confirmed = 1) = 1 
    THEN
      UPDATE `members`
        SET `member_email`=p_member_email,`member_first_name`=p_member_first_name,`member_last_name`=p_member_last_name,`member_reg_number`=p_member_reg_number,`member_create_date`=p_member_create_date,`member_update_date`=p_member_update_date,`member_country`=p_member_country,`member_state`=p_member_state,`member_gender`=p_member_gender,`member_mobile_phone`=p_member_mobile_phone,`member_mobile_carrier`=p_member_mobile_carrier,`member_city`=p_member_city,`member_zip`=p_member_zip,`member_address`=p_member_address,`member_address2`=p_member_address2,`member_order_number`=p_member_order
        WHERE (`member_id` = p_member_id); -- SQLINES DEMO ***  = @RowVersion);
      
      SELECT * FROM `members` WHERE `member_id` = p_member_id;
    END IF;
END;


Create PROCEDURE `usp_tbl_member_ups`
  (p_member_email nvarchar(256),p_member_code nvarchar(512),p_member_first_name nvarchar(25),p_member_last_name nvarchar(25),p_member_reg_number nvarchar(30),p_member_create_date datetime(3),p_member_update_date datetime(3),p_ID bigint/* =NULL */,p_member_country nvarchar(30)/* =NULL */,p_member_state nvarchar(25)/* =NULL */,p_member_gender nvarchar(20)/* =NULL */,p_member_mobile_phone nvarchar(20)/* =NULL */,p_member_mobile_carrier nvarchar(25)/* =NULL */,p_member_city nvarchar(30)/* =NULL */,p_member_zip nvarchar(10)/* =NULL */,p_member_address nvarchar(50)/* =NULL */,p_member_address2 nvarchar(50)/* =NULL */,p_member_order nvarchar(50)/* =NULL */)
BEGIN
  IF (p_ID IS NULL OR p_ID = 0) and (Select COUNT(*) from members where member_email = p_member_email and member_enabled = 1 and member_confirmed = 1) = 0 
   
    THEN
      
      INSERT INTO `members`
        (`member_email`,`member_code`,`member_first_name`,`member_last_name`,`member_reg_number`,`member_create_date`,`member_update_date`,`member_country`,`member_state`,`member_gender`,`member_mobile_phone`,`member_mobile_carrier`,`member_city`,`member_zip`,`member_address`,`member_address2`,`member_order_number`)
      VALUES
       (p_member_email,p_member_code,p_member_first_name,p_member_last_name,p_member_reg_number,p_member_create_date,p_member_update_date,p_member_country,p_member_state,p_member_gender,p_member_mobile_phone,p_member_mobile_carrier,p_member_city,p_member_zip,p_member_address,p_member_address2,p_member_order);
      
      SELECT * FROM `members` WHERE `member_id` = LAST_INSERT_ID();
  ELSE
      UPDATE `members`
        SET `member_email`=p_member_email,`member_code`=p_member_code,`member_first_name`=p_member_first_name,`member_last_name`=p_member_last_name,`member_reg_number`=p_member_reg_number,`member_create_date`=p_member_create_date,`member_update_date`=p_member_update_date,`member_country`=p_member_country,`member_state`=p_member_state,`member_gender`=p_member_gender,`member_mobile_phone`=p_member_mobile_phone,`member_mobile_carrier`=p_member_mobile_carrier,`member_city`=p_member_city,`member_zip`=p_member_zip,`member_address`=p_member_address,`member_address2`=p_member_address2,`member_order_number`=p_member_order
        WHERE (`member_id` = p_ID); -- SQLINES DEMO ***  = @RowVersion);
      
      SELECT * FROM `members` WHERE `member_id` = p_ID;
    END IF;
END;

 

 



