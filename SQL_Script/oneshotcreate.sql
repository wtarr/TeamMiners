---------------------------------------
-- Drop previous 
---------------------------------------

PROMPT ### BEGIN SCRIPT ###
PROMPT ### Removing Sequences ###
-- Sequence for auto generating userids
DROP SEQUENCE userIDAUTOSEQ;

-- Sequence for auto generation itemid
DROP SEQUENCE itemIDAUTOSEQ;

-- Sequence for auto generating inventory ids
DROP sequence invIDAUTOSEQ;

-- Sequence for auto generating tool ids
DROP SEQUENCE toolIDAUTOSEQ;

-- Sequence for auto generation build id
DROP SEQUENCE buildingIDAUTOSEQ;

PROMPT ### Removing Indexes ###
-- PROMPT Removing Index
drop index inventory_itemname_index;
drop index stackitem_itemname_index;

PROMPT ### Removing Views ###
DROP view secureUserView;
DROP view usersbuildings;
DROP view usersinventory;

PROMPT ### Drop Triggers ###
DROP trigger MineItemRequirements;
DROP trigger passwordCheck;
DROP trigger updateUsersPos;
DROP trigger toolInvReqChk;

DROP trigger updateInvToolID;
DROP trigger buildInvReqChk;
DROP trigger updateInvBuildingID;
DROP trigger chkItemIsMarkedShare;

-- ::::::Drop table::::::::
PROMPT ### Removing Constraints ###
-- UserAccount
ALTER TABLE useraccount DROP CONSTRAINT XYcoord_tblMapcell_fk;

-- MapCell
ALTER TABLE mapcell DROP CONSTRAINT UserID_tblMapCell_fk;
ALTER TABLE mapcell DROP CONSTRAINT BuildingID_tblMapCell_fk;

-- StackItem
ALTER TABLE stackitem DROP CONSTRAINT XYcoord_tblStkItem_fk;
ALTER TABLE stackitem DROP CONSTRAINT userid_tblStkItem_fk;

-- InventoryItem
ALTER TABLE inventoryitem DROP CONSTRAINT userid_tblInvItem_fk;
ALTER TABLE inventoryitem DROP CONSTRAINT toolID_tblInvItem_fk;
ALTER TABLE inventoryitem DROP CONSTRAINT bldID_tblInvItem_fk;

-- Tool
ALTER TABLE tool DROP CONSTRAINT userid_tblTool_fk;

-- Building
ALTER TABLE building DROP CONSTRAINT userid_tblBld_fk;

-- SharedItem
ALTER TABLE shareditem DROP CONSTRAINT userid_tblshare_fk; 
ALTER TABLE shareditem DROP CONSTRAINT inventid_tblshare_fk; 

PROMPT ### Dropping Tables ###
DROP TABLE useraccount;
DROP TABLE mapcell;
DROP TABLE stackitem;
DROP TABLE inventoryitem;
DROP TABLE shareditem;
DROP TABLE tool;
DROP TABLE building;

PROMPT ### Create Tables ###
CREATE TABLE useraccount(
userskin BLOB,
userid NUMBER(5),
username VARCHAR2(20) NOT NULL UNIQUE,
password VARCHAR2(20),
onlinestatus NUMBER(1) CHECK (onlinestatus IN (0, 1)),
xcoord NUMBER(3),
ycoord NUMBER(3),
CONSTRAINT userid_tblUserAccount_pk PRIMARY KEY(userid)
);


CREATE TABLE mapcell(
xcoord NUMBER(3),
ycoord NUMBER(3),
currentblock number(1), -- made a number instead of varchar
userid NUMBER(5),
buildingid NUMBER(3),
CONSTRAINT XY_tblMapCell_pk PRIMARY KEY(xcoord, ycoord)
);

CREATE TABLE stackitem(
itemid number(5),
itemorder NUMBER(3),
itemname VARCHAR2(20),
toolrequired NUMBER(1) CHECK (toolrequired IN (0, 1)),
xcoord NUMBER(3),
ycoord NUMBER(3),
userid NUMBER(5),
--toolid NUMBER(5),
CONSTRAINT itemid_tblStackItem_pk PRIMARY KEY(itemid)
);

CREATE TABLE inventoryitem(
inventoryid NUMBER(3),
itemname VARCHAR2(20),
issharable NUMBER(1) CHECK (issharable IN (0, 1)),
userid NUMBER(5) NOT NULL,
toolid NUMBER(3),
buildingid NUMBER(3),
CONSTRAINT InvID_tblInventoryItem_pk PRIMARY KEY(inventoryid)
);

CREATE TABLE tool(
toolid NUMBER(3),
--toolowner VARCHAR2(30), 
toolname VARCHAR2(30),
toollife NUMBER(2),
userid NUMBER(5),
CONSTRAINT toolid_tblTool_pk PRIMARY KEY(toolid)
);

CREATE TABLE shareditem(
userid NUMBER(5),
inventoryid NUMBER(3),
sharedwithuser NUMBER(5) NOT NULL,
CONSTRAINT UserIDInvID_tblSrdItem_pk PRIMARY KEY(userid, inventoryid)
);

CREATE TABLE building(
buildingid NUMBER(3),
length NUMBER(1),
width NUMBER(1),
--owner NUMBER(5), 
forsale NUMBER(1) CHECK (forsale IN (0, 1)),
userid NUMBER(5),
xcoord NUMBER(3) NOT NULL,
ycoord NUMBER(3) NOT NULL,
CONSTRAINT bldID_tblBld_pk PRIMARY KEY(buildingid)
);

PROMPT ### Adding Foregin keys ###
-- Alter tables to add Foregin keys

-- UserAccount
ALTER TABLE useraccount ADD CONSTRAINT XYcoord_tblMapcell_fk FOREIGN KEY(xcoord, ycoord) REFERENCES mapcell ON DELETE CASCADE;

-- MapCell
ALTER TABLE mapcell ADD CONSTRAINT UserID_tblMapCell_fk FOREIGN KEY(userid) REFERENCES useraccount ON DELETE CASCADE;
ALTER TABLE mapcell ADD CONSTRAINT BuildingID_tblMapCell_fk FOREIGN KEY(buildingid) REFERENCES building ON DELETE CASCADE;

-- Stack Item
ALTER TABLE stackitem ADD CONSTRAINT XYcoord_tblStkItem_fk FOREIGN KEY(xcoord, ycoord) REFERENCES mapcell ON DELETE CASCADE;
ALTER TABLE stackitem ADD CONSTRAINT userid_tblStkItem_fk FOREIGN KEY(userid) REFERENCES useraccount ON DELETE CASCADE;

-- Inventory Item
ALTER TABLE inventoryitem ADD CONSTRAINT userid_tblInvItem_fk FOREIGN KEY(userid) REFERENCES useraccount ON DELETE CASCADE;
ALTER TABLE inventoryitem ADD CONSTRAINT toolID_tblInvItem_fk FOREIGN KEY(toolid) REFERENCES tool ON DELETE CASCADE;
ALTER TABLE inventoryitem ADD CONSTRAINT bldID_tblInvItem_fk FOREIGN KEY(buildingid) REFERENCES building ON DELETE CASCADE;

-- Tool
ALTER TABLE tool ADD CONSTRAINT userid_tblTool_fk FOREIGN KEY(userid) REFERENCES useraccount ON DELETE CASCADE;

-- Building
ALTER TABLE building ADD CONSTRAINT userid_tblBld_fk FOREIGN KEY(userid) REFERENCES useraccount ON DELETE CASCADE;

-- SharedItem
ALTER TABLE shareditem ADD CONSTRAINT userid_tblshare_fk FOREIGN KEY(userid) REFERENCES useraccount ON DELETE CASCADE;
ALTER TABLE shareditem ADD CONSTRAINT inventid_tblshare_fk FOREIGN KEY(inventoryid) REFERENCES inventoryitem ON DELETE CASCADE;


PROMPT ### Create Sequences ###
-- Sequence for auto generating userids
CREATE SEQUENCE userIDAUTOSEQ start with 1 NOCACHE;

-- Sequence for auto generation itemid
CREATE SEQUENCE itemIDAUTOSEQ start with 1 NOCACHE;

-- Sequence for auto generating inventory ids
CREATE sequence invIDAUTOSEQ start with 1 NOCACHE;

-- Sequence for auto generating tool ids
CREATE SEQUENCE toolIDAUTOSEQ start with 1 NOCACHE;

-- Sequence for auto generation build id
CREATE SEQUENCE buildingIDAUTOSEQ start with 1 NOCACHE;

PROMPT ### Create indexes ###
-- Create the indexes
CREATE INDEX inventory_itemname_index
on inventoryitem(itemname);

CREATE INDEX stackitem_itemname_index
on stackitem(itemname);

PROMPT ### Create views ###
-- show the users without revealing their account number or password.
CREATE OR REPLACE VIEW secureUserView AS
SELECT UserName, OnlineStatus, XCoord, YCoord
FROM useraccount;

-- show the users name along with the buildings they own
CREATE OR replace view usersbuildings AS
select username, buildingid, length, width, building.xcoord, building.ycoord
FROM useraccount, building
WHERE useraccount.userid = building.userid;

-- 
CREATE OR REPLACE view usersinventory AS
SELECT username, itemname, issharable
FROM useraccount, inventoryitem
WHERE useraccount.userid = inventoryitem.userid;
---------------------------
-- Add the triggers
---------------------------

PROMPT ### Adding Triggers ###

create or replace TRIGGER MineItemRequirements
-- Check what the user is actually mining, if Lava, delete all their inventory else add the item to their
-- Inventory and updates the mapcells current block index
-- Also a check is performed to see if a tool is required and if so its toollife is updated upon use.
-- user is trying to take the item by associating his id with it
BEFORE UPDATE OF userid ON stackitem 
FOR EACH ROW
DECLARE
  v_num stackitem.itemorder%type;  
  v_toolid tool.toolid%type;
  v_toollife tool.toollife%type;
BEGIN
 IF (:old.itemname LIKE ('Lava') AND :old.userid IS NULL) THEN
     BEGIN
       --Delete inventory
       DBMS_OUTPUT.PUT_LINE('That was Lava - You lose all your inventory');
       delete inventoryitem WHERE userid = :new.userid AND toolid IS NULL AND buildingid IS NULL;
     END;
ELSE
    BEGIN
      -- Insert it into inventory
      IF (:old.itemname NOT LIKE ('Lava') AND :old.itemname NOT LIKE ('Building') AND :old.userid IS NULL) THEN
        IF (:old.toolrequired = 1) THEN 
        -- A tool is needed stone or iron pick
          BEGIN                   
              select * into v_toolid from -- Select first that satisfies criteria and place in holder
              (
                select toolid from tool where (toolname LIKE 'StonePick' OR toolname LIKE 'IronPick') AND toollife > 0 AND userid = :new.userid
              ) where ROWnum <= 1;
            EXCEPTION
              WHEN OTHERS THEN          
                RAISE_APPLICATION_ERROR(-20008, 'No Data found when search performed on tool table');
            END;
          DBMS_OUTPUT.PUT_LINE('ToolID found = '||v_toolid);          
          select toollife into v_toollife from tool where toolid = v_toolid;
          IF (v_toolid IS NULL) THEN
            RAISE_APPLICATION_ERROR(-20007, 'You possess no tool to mine this item');
          ELSE
            -- deduct 1 life from that tool
            v_toollife := v_toollife - 1;
            update tool set toollife = v_toollife where toolid = v_toolid;
            -- transfer the item to the inventory
      -- and update the current block field in mapcell with the new item order
            v_num := :old.itemorder;
            v_num := v_num + 1;
            insert into inventoryitem values (invIDAUTOSEQ.NEXTVAL, :old.itemname, 0, :new.userid, null, null);            
            update mapcell set currentblock = v_num where xcoord = :old.xcoord and xcoord = :old.ycoord;
          END IF;
        ELSE
          -- No tool reqd add to inventory and update the mapcells current item block
          v_num := :old.itemorder;
          v_num := v_num + 1;
          insert into inventoryitem values (invIDAUTOSEQ.NEXTVAL, :old.itemname, 0, :new.userid, null, null);
          update mapcell set currentblock = v_num where xcoord = :old.xcoord and xcoord = :old.ycoord;
        END IF;
      END IF;
      END;
END IF;
END;
/

create or replace TRIGGER passwordCheck
-- When creating a new account check that the users password is at least 8 characters long and contains a digit
BEFORE INSERT ON useraccount
FOR EACH ROW
BEGIN
IF (REGEXP_LIKE(:new.password, '[0-9]') AND LENGTH(:new.password) >= 8) THEN
    -- Ok to insert
    DBMS_OUTPUT.PUT_LINE('Password OK');
ELSE
    -- Don’t insert throw error!!
    RAISE_APPLICATION_ERROR(-20000, 'Password needs to be at least 8 characters long and contain at least 1 digit');
END IF;
END;
/


create or replace TRIGGER updateUsersPos
-- Update the users position when the move to a new map cell
AFTER UPDATE OR INSERT ON mapcell
FOR EACH ROW
BEGIN
  --dbms_output.put_line('ID ' || :new.userid);
  update useraccount set xcoord = :new.xcoord where userid = :new.userid and :new.currentblock > 0;
  update useraccount set ycoord = :new.ycoord where userid = :new.userid and :new.currentblock > 0;
END;
/


create or replace TRIGGER toolInvReqChk
-- Check that the user has the required 
-- inventory to create the tool in question
BEFORE INSERT ON tool
FOR EACH ROW
DECLARE
  v_wood_usersStock number(2); -- Store how much wood the user has
  v_stone_usersStock number(2); -- Store how much stone the user has
  v_iron_usersStock number(2); -- Store how much iron the user has
  v_wood_requirement number(1); -- wood for handle
  v_stone_requirement number(1); -- stone for the head (the business end of the tool)
  v_iron_requirement number(1); -- Iron for the head 
BEGIN 
  IF (:new.toolname LIKE 'StoneHammer') THEN
      BEGIN
      -- The req is 2 wood and 1 stone
      v_wood_requirement := 2;
      v_stone_requirement := 1;
      
      SELECT COUNT(*) 
      INTO v_wood_usersStock
      FROM inventoryItem 
      WHERE userid = :new.userid and
      itemname LIKE 'Wood' and
      toolid IS NULL and
      issharable = 0 and 
      buildingid is NULL;

      SELECT COUNT(*) 
      INTO v_stone_usersStock
      FROM inventoryItem 
      WHERE userid = :new.userid and
      itemname LIKE 'Stone' and
      toolid IS NULL and
      issharable = 0 and 
      buildingid is NULL;

      IF (v_wood_usersStock < v_wood_requirement OR v_stone_usersStock < v_stone_requirement) THEN       
        RAISE_APPLICATION_ERROR(-20001, 'Instufficent inventory to make stone hammer tool');
      END IF;
      END;
  ELSIF (:new.toolname LIKE 'StonePick') THEN
      BEGIN
      -- The requirement is 2 wood and 3 stone
      v_wood_requirement := 2;
      v_stone_requirement := 3;

      SELECT COUNT(*) 
      INTO v_wood_usersStock
      FROM inventoryItem 
      WHERE userid = :new.userid and
      itemname LIKE 'Wood' and
      toolid IS NULL and
      issharable = 0 and 
      buildingid is NULL;

      SELECT COUNT(*) 
      INTO v_stone_usersStock
      FROM inventoryItem 
      WHERE userid = :new.userid and
      itemname LIKE 'Stone' and
      toolid IS NULL and
      issharable = 0 and 
      buildingid is NULL;

      IF (v_wood_usersStock < v_wood_requirement OR v_stone_usersStock < v_stone_requirement) THEN
        RAISE_APPLICATION_ERROR(-20002, 'Instufficent inventory to make stone Pick tool');
      END IF;
      END;

  ELSIF (:new.toolname LIKE 'IronHammer') THEN
      BEGIN
      -- The requirement is 2 wood and 1 iron
      v_wood_requirement := 2;
      v_iron_requirement := 1;

      SELECT COUNT(*) 
      INTO v_wood_usersStock
      FROM inventoryItem 
      WHERE userid = :new.userid and
      itemname LIKE 'Wood' and
      toolid IS NULL and
      issharable = 0 and 
      buildingid is NULL;

      SELECT COUNT(*) 
      INTO v_iron_usersStock
      FROM inventoryItem 
      WHERE userid = :new.userid and
      itemname LIKE 'Iron' and
      toolid IS NULL and
      issharable = 0 and 
      buildingid is NULL;

      IF (v_wood_usersStock < v_wood_requirement OR v_iron_usersStock < v_iron_requirement) THEN
        RAISE_APPLICATION_ERROR(-20002, 'Instufficent inventory to make iron hammer tool');
      END IF;


    END;
  ELSIF (:new.toolname LIKE 'IronPick') THEN
      BEGIN
      -- The requirement is 2 wood and 3 iron
      v_wood_requirement := 2;
      v_iron_requirement := 3;

      SELECT COUNT(*) 
      INTO v_wood_usersStock
      FROM inventoryItem 
      WHERE userid = :new.userid and
      itemname LIKE 'Wood' and
      toolid IS NULL and
      issharable = 0 and 
      buildingid is NULL;

      SELECT COUNT(*) 
      INTO v_iron_usersStock
      FROM inventoryItem 
      WHERE userid = :new.userid and
      itemname LIKE 'Iron' and
      toolid IS NULL and
      issharable = 0 and 
      buildingid is NULL;

      IF (v_wood_usersStock < v_wood_requirement OR v_iron_usersStock < v_iron_requirement) THEN      
        RAISE_APPLICATION_ERROR(-20003, 'Instufficent inventory to make iron pick tool');
      END IF;
      END;
  END IF;  
  
END;
/


create or replace TRIGGER updateInvToolID
-- Update the inventory and associate the toolid
-- to the inventory that was used to create it
AFTER INSERT ON tool
FOR EACH ROW
DECLARE 
  v_index inventoryitem.inventoryid%type;
BEGIN 

  -- loop thru inventory and update 2 wood as regardless all tools use 2 wood for handle
    FOR loopctr1 IN 1..2
    LOOP
        select * into v_index from -- Select first that satisfies criteria and place in holder
        (
          select inventoryid from inventoryitem where itemname LIKE 'Wood' and userid = :new.userid and toolid is null and buildingid is null
        )
        where ROWnum <= 1;      
      update inventoryitem set toolid = :new.toolid where inventoryid = v_index;
    END LOOP;

  IF (:new.toolname LIKE 'StoneHammer') THEN
      dbms_output.put_line('ID ' || :new.toolid);    
    
    -- loop thru inventory and update 1 stone item ONLY!!!
    FOR loopctr2 IN 1..1
    LOOP
      select * into v_index from -- Select first that satisfies criteria and place in holder
        (
          select inventoryid from inventoryitem where itemname LIKE 'Stone' and userid = :new.userid and toolid is null and buildingid is null
        )
        where ROWnum <= 1;  
      update inventoryitem set toolid = :new.toolid where inventoryid = v_index;
    END LOOP;

  ELSIF (:new.toolname LIKE 'StonePick') THEN -- StonePick consumes 2 wood and 3 Stone
      dbms_output.put_line('ID ' || :new.toolid);     
    
    -- loop thru inventory and update 3 Stone ONLY!
    FOR loopctr2 IN 1..3
    LOOP
      select * into v_index from -- Select first that satisfies criteria and place in holder
        (
          select inventoryid from inventoryitem where itemname LIKE 'Stone' and userid = :new.userid and toolid is null and buildingid is null
        )
        where ROWnum <= 1;  
      update inventoryitem set toolid = :new.toolid where inventoryid = v_index;
    END LOOP;


  ELSIF (:new.toolname LIKE 'IronHammer') THEN -- Iron hammer consumes 2 wood and 1 iron
      dbms_output.put_line('ID ' || :new.toolid);     
    
    -- loop thru inventory and update 1 ONLY 
    FOR loopctr2 IN 1..1
    LOOP
      select * into v_index from -- Select first that satisfies criteria and place in holder
        (
          select inventoryid from inventoryitem where itemname LIKE 'Iron' and userid = :new.userid and toolid is null and buildingid is null
        )
        where ROWnum <= 1;  
      update inventoryitem set toolid = :new.toolid where inventoryid = v_index;
    END LOOP;


  ELSIF (:new.toolname LIKE 'IronPick') THEN -- Iron pick consumes 2 wood and 3 iron
    dbms_output.put_line('ID ' || :new.toolid);     
    
    -- loop thru inventory and update 3 
    FOR loopctr2 IN 1..3
    LOOP
      select * into v_index from -- Select first that satisfies criteria and place in holder
        (
          select inventoryid from inventoryitem where itemname LIKE 'Iron' and userid = :new.userid and toolid is null and buildingid is null
        )
        where ROWnum <= 1;  
      update inventoryitem set toolid = :new.toolid where inventoryid = v_index;
    END LOOP;    
  END IF;  
  
END;
/


create or replace TRIGGER buildInvReqChk
-- Check that the required inventory is available to build the house and that the 
-- location is free to place it there i.e. x, y
BEFORE INSERT ON building
FOR EACH ROW
DECLARE
  v_wood_usersStock number(2); -- Store how much wood the user has
  v_stone_usersStock number(2); -- Store how much stone the user has  
  v_wood_requirement number(2); -- wood for roof
  v_stone_requirement number(2); -- stone for the blocks
  v_toolid tool.toolid%type; 
  v_toollife_actual tool.toollife%type;  
  v_toollife_requirement number(2); --tool req of at least ten
BEGIN 
        -- The req is 10 wood and 10 stone
        v_wood_requirement := 10;
        v_stone_requirement := 12;
        v_toollife_requirement := 10;
        
        -- Count the wood in the users inventory
        SELECT COUNT(*) 
        INTO v_wood_usersStock
        FROM inventoryItem 
        WHERE userid = :new.userid and
        itemname LIKE 'Wood' and
        toolid IS NULL and
        issharable = 0 and 
        buildingid is NULL;
        -- Count the stone in the users inventory
        SELECT COUNT(*) 
        INTO v_stone_usersStock
        FROM inventoryItem 
        WHERE userid = :new.userid and
        itemname LIKE 'Stone' and
        toolid IS NULL and
        issharable = 0 and 
        buildingid is NULL;

        BEGIN                   
              select * into v_toolid from -- Select first that satisfies criteria and place in holder
              (
                select toolid from tool where toolname LIKE 'StoneHammer' OR toolname LIKE 'IronHammer' AND toollife >= v_toollife_requirement
              ) where ROWnum <= 1;
        EXCEPTION
              WHEN OTHERS THEN          
                RAISE_APPLICATION_ERROR(-20008, 'No Data found when search performed on tool table');
        END;
             
        DBMS_OUTPUT.PUT_LINE('WOOD '||v_wood_usersStock ||' Stone '||v_stone_usersStock);
        IF (v_wood_usersStock < v_wood_requirement OR v_stone_usersStock < v_stone_requirement) THEN      
          RAISE_APPLICATION_ERROR(-20004, 'Instufficent inventory to make building');
        ELSE
            -- Update the mapcell now, this will fail if xy is occupied
            BEGIN                   
              INSERT INTO mapcell values(:new.xcoord, :new.ycoord, 0, :new.userid, null);
              -- Update toollife
              select toollife into v_toollife_actual from tool where toolid = v_toolid;
              v_toollife_actual := v_toollife_actual - 10;
              update tool set toollife = v_toollife_actual where toolid = v_toolid;
            EXCEPTION
              WHEN OTHERS THEN          
                RAISE_APPLICATION_ERROR(-20006, 'That cell is occupied');
            END;   
        END IF;
                    
END;
/


create or replace TRIGGER updateInvBuildingID
--Update the inventories building id depending on what was used
AFTER INSERT ON building
FOR EACH ROW
DECLARE 
  v_index inventoryitem.inventoryid%type;
BEGIN 
    update MapCell set buildingid = :new.buildingid where xcoord = :new.xcoord and ycoord = :new.ycoord;

    -- The roof uses 10 wood
   FOR loopctr11 IN 1..10
    LOOP
        select * into v_index from -- Select first that satisfies criteria and place in holder
        (
          select inventoryid from inventoryitem where itemname LIKE 'Wood' and userid = :new.userid and toolid is null and buildingid is null
        )
        where ROWnum <= 1;      
      update inventoryitem set buildingid = :new.buildingid where inventoryid = v_index;
    END LOOP;
    -- Walls use 12 stones
     FOR loopctr12 IN 1..12
    LOOP
        select * into v_index from -- Select first that satisfies criteria and place in holder
        (
          select inventoryid from inventoryitem where itemname LIKE 'Stone' and userid = :new.userid and toolid is null and buildingid is null
        )
        where ROWnum <= 1;      
      update inventoryitem set buildingid = :new.buildingid where inventoryid = v_index;
    END LOOP;    
END;
/


create or replace TRIGGER chkItemIsMarkedShare
-- Check that the inventory item is marked sharable before inserting in share table
BEFORE INSERT ON shareditem
FOR EACH ROW
DECLARE
  v_res number(1);
  v_toolid inventoryitem.toolid%type;
  v_buildingid inventoryitem.toolid%type;
BEGIN
  SELECT issharable, toolid, buildingid INTO v_res, v_toolid, v_buildingid FROM inventoryitem WHERE inventoryid = :new.inventoryid;
  IF (v_res != 1 OR v_toolid IS NOT NULL OR v_buildingid IS NOT NULL) THEN
    RAISE_APPLICATION_ERROR(-20005, 'That item is not sharable');
  END IF;
END;
/
 

PROMPT ### Done! ###