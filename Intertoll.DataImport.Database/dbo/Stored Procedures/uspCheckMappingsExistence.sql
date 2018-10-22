

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE uspCheckMappingsExistence
AS
BEGIN
	SET NOCOUNT ON;

    IF(NOT EXISTS(SELECT * FROM MappingLanes))
	BEGIN
		/*DECLARE @message VARCHAR(1000) = 'Lane mappings not yet created' 
		RAISERROR(@message,20,-1) WITH LOG*/

		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'01RN', N'09420163-09ff-4036-810a-5eea96bf8406', N'149', N'11')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'02RN', N'1b393384-5ff2-416e-9723-8c3769dd1778', N'149', N'11')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'03RS', N'c896c0e6-afde-47c0-93c5-bddd1a7a9868', N'148', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'04RN', N'f29d6701-d40f-4e79-bc86-f0de4c05207a', N'147', N'11')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'04RS', N'b32f557d-cad1-4513-ba79-7e1ea3c394d2', N'148', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'05RN', N'26c8b0b0-8a4e-42eb-9e4c-a980e8bf571f', N'147', N'11')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'07MN', N'62b4c963-f2c6-4029-8c9e-24f61f93cf3c', N'145', N'11')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'08MN', N'62e08e0e-9563-41f4-9460-bb91546e8c60', N'145', N'11')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'09MN', N'49de85ba-58fe-4d8f-81e7-fde9e3dcf4e8', N'145', N'11')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'10MN', N'4098b23c-306f-4030-9bd2-b934b042801f', N'145', N'11')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'11MN', N'e7c9c129-a01d-4a80-b8a1-07457c37ff1d', N'145', N'11')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'12MS', N'17975746-b4d7-4c92-98bc-f40edc8442d6', N'144', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'13MS', N'a18713a7-4254-486c-9eca-4d806daf3dea', N'144', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'14MS', N'e6124ae5-db80-4d02-8b81-d0837820cde0', N'144', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'15MS', N'1dc0f85d-0902-4c69-97da-327f34efab98', N'144', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'16MS', N'8cc789e3-d2cc-4e8c-9bd5-288461149e04', N'144', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'18RS', N'ce168fd5-9f89-40f8-b7e6-2e9c595311a4', N'146', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'19RS', N'4f81627c-d02b-45d2-ab5c-5321233d0797', N'146', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'81RS', N'997f74df-df95-4d60-aa2b-8b57fd14e7ed', N'150', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'82RS', N'9f7e39be-3174-4ea4-a56f-ffaf83fe3f51', N'150', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'83RS', N'07498f20-03e9-4bb8-8e26-b6ce85d06f82', N'150', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'84RS', N'6187db29-6c93-4746-acac-b70cc4ed2616', N'150', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'85RS', N'4d3d1455-0ccd-45b2-be12-7ef5ec02bf7b', N'150', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'86RS', N'fa0403c6-af26-4c98-9c09-a2a92791c794', N'150', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'87RS', N'df6deeb9-d592-4c4b-b1e6-d13ad6103b4d', N'150', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'88RS', N'3dc1a3fc-0f0b-4e49-a8ca-063165878578', N'150', N'12')
		
		INSERT [dbo].[MappingLanes] ([LaneCode], [LaneGuid], [VPCode], [ETCDirectionCode]) VALUES (N'89RS', N'927e2f78-3b2a-4ac0-84cd-6e81884fe48c', N'150', N'12')
		
	END

	IF(NOT EXISTS(SELECT * FROM MappingTariffs))
	BEGIN
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'8c98029d-f924-4930-807c-0384850be7f5', N'TA', 1, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'328f9b18-d423-4165-b838-173e220b706b', N'TR', 3, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'8cf4ed50-b315-48b5-94bd-2038e024b92a', N'TM', 5, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'24305942-c367-4721-b554-2dd0e1c460ba', N'TA', 5, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'ddeb997e-e814-4230-a747-3594a3827379', N'TA', 4, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'7fe55c4c-6bc5-486a-9c88-3738ec1b8be5', N'TL', 1, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'c283a3d7-fcdc-4762-a3f1-4bd446bf2a0e', N'TL', 5, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'7d0c6c5d-b5dc-4095-b081-50f2ea51ab6e', N'TM', 3, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'41ee0bb4-46cd-463b-9849-510d7b0a61f9', N'TL', 2, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'09c52152-2c24-41cc-af75-73c8586ee1e4', N'TA', 2, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'35d1c15a-b769-4771-b51f-7f0f55ec6013', N'TR', 5, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'77bce529-0a01-489d-bca3-9dc4257d719b', N'TM', 4, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'0f64c408-779a-4999-81bd-b18554b3f2f6', N'TA', 3, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'3a7457d8-24fb-4df9-9add-b57fb1b9cfb9', N'TR', 4, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'7d95edb0-f4b2-4d1f-89ca-b68eeb7dc7ca', N'TL', 3, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'7c69c241-ff1f-486a-943f-cdf1b32f96a1', N'TM', 1, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'287ef6dc-7fb6-4223-8f7a-dbd877ebc22b', N'TR', 1, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'8718eaff-5c8c-466b-ac93-dff29d415838', N'TM', 2, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'bd7a38e2-192c-42da-85b3-e8e894367afe', N'TL', 4, 25)
		
		INSERT [dbo].[MappingTariffs] ([TariffGuid], [VirtualPlaza], [Class], [TariffTableID]) VALUES (N'ab52bbf8-1420-4e93-b170-fd7fe33abbf0', N'TR', 2, 25)
		
	END

	IF(NOT EXISTS(SELECT * FROM MappingIncidentTypes))
	BEGIN
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'ca60a87f-8f5d-df11-934b-001517c991cf', N'', 10000)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'036c8ae2-e700-df11-ad4b-001517c991cf', N'SAL', 1)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'046c8ae2-e700-df11-ad4b-001517c991cf', N'SH', 2)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'056c8ae2-e700-df11-ad4b-001517c991cf', N'SI', 3)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'066c8ae2-e700-df11-ad4b-001517c991cf', N'UM', 4)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'076c8ae2-e700-df11-ad4b-001517c991cf', N'UJ', 5)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'086c8ae2-e700-df11-ad4b-001517c991cf', N'UN', 6)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'096c8ae2-e700-df11-ad4b-001517c991cf', N'OZ', 7)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'0a6c8ae2-e700-df11-ad4b-001517c991cf', N'', 8)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'0b6c8ae2-e700-df11-ad4b-001517c991cf', N'', 9)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'0c6c8ae2-e700-df11-ad4b-001517c991cf', N'', 10)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'0d6c8ae2-e700-df11-ad4b-001517c991cf', N'', 11)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'0e6c8ae2-e700-df11-ad4b-001517c991cf', N'SBH', 12)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'0f6c8ae2-e700-df11-ad4b-001517c991cf', N'SBI', 13)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'106c8ae2-e700-df11-ad4b-001517c991cf', N'SBF', 14)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'116c8ae2-e700-df11-ad4b-001517c991cf', N'SBG', 15)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'126c8ae2-e700-df11-ad4b-001517c991cf', N'SAD', 16)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'136c8ae2-e700-df11-ad4b-001517c991cf', N'SEB', 17)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'146c8ae2-e700-df11-ad4b-001517c991cf', N'', 18)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'156c8ae2-e700-df11-ad4b-001517c991cf', N'', 19)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'166c8ae2-e700-df11-ad4b-001517c991cf', N'', 20)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'176c8ae2-e700-df11-ad4b-001517c991cf', N'', 21)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'186c8ae2-e700-df11-ad4b-001517c991cf', N'', 22)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'196c8ae2-e700-df11-ad4b-001517c991cf', N'', 23)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'1a6c8ae2-e700-df11-ad4b-001517c991cf', N'', 24)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'1b6c8ae2-e700-df11-ad4b-001517c991cf', N'', 26)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'1c6c8ae2-e700-df11-ad4b-001517c991cf', N'', 27)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'1d6c8ae2-e700-df11-ad4b-001517c991cf', N'SCQ', 34)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'1e6c8ae2-e700-df11-ad4b-001517c991cf', N'UL', 41)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'1f6c8ae2-e700-df11-ad4b-001517c991cf', N'OP', 42)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'206c8ae2-e700-df11-ad4b-001517c991cf', N'', 43)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'216c8ae2-e700-df11-ad4b-001517c991cf', N'SCV', 44)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'226c8ae2-e700-df11-ad4b-001517c991cf', N'SCF', 46)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'236c8ae2-e700-df11-ad4b-001517c991cf', N'SCG', 47)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'246c8ae2-e700-df11-ad4b-001517c991cf', N'SCT', 54)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'256c8ae2-e700-df11-ad4b-001517c991cf', N'SCU', 55)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'266c8ae2-e700-df11-ad4b-001517c991cf', N'SCZ', 33)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'276c8ae2-e700-df11-ad4b-001517c991cf', N'', 35)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'286c8ae2-e700-df11-ad4b-001517c991cf', N'', 37)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'296c8ae2-e700-df11-ad4b-001517c991cf', N'', 38)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'2a6c8ae2-e700-df11-ad4b-001517c991cf', N'', 39)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'2b6c8ae2-e700-df11-ad4b-001517c991cf', N'', 40)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'2c6c8ae2-e700-df11-ad4b-001517c991cf', N'', 48)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'2d6c8ae2-e700-df11-ad4b-001517c991cf', N'', 49)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'2e6c8ae2-e700-df11-ad4b-001517c991cf', N'', 50)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'2f6c8ae2-e700-df11-ad4b-001517c991cf', N'', 51)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'306c8ae2-e700-df11-ad4b-001517c991cf', N'', 52)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'316c8ae2-e700-df11-ad4b-001517c991cf', N'', 53)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'326c8ae2-e700-df11-ad4b-001517c991cf', N'SCW', 56)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'336c8ae2-e700-df11-ad4b-001517c991cf', N'SCX', 57)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'346c8ae2-e700-df11-ad4b-001517c991cf', N'', 58)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'356c8ae2-e700-df11-ad4b-001517c991cf', N'', 25)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'366c8ae2-e700-df11-ad4b-001517c991cf', N'', 28)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'376c8ae2-e700-df11-ad4b-001517c991cf', N'', 29)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'386c8ae2-e700-df11-ad4b-001517c991cf', N'SCR', 30)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'396c8ae2-e700-df11-ad4b-001517c991cf', N'SCL', 31)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'3a6c8ae2-e700-df11-ad4b-001517c991cf', N'SCY', 32)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'3b6c8ae2-e700-df11-ad4b-001517c991cf', N'SCS', 45)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'a0edde4e-ab26-df11-ba8b-001517c991cf', N'', 1000)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'5047ff60-ab26-df11-ba8b-001517c991cf', N'', 1001)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'70a24c7a-ab26-df11-ba8b-001517c991cf', N'', 1002)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'3001a02d-ae26-df11-ba8b-001517c991cf', N'', 1003)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'd1e31f77-bc26-df11-ba8b-001517c991cf', N'', 1004)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'b0872692-bc26-df11-ba8b-001517c991cf', N'', 1005)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'000bc09d-bc26-df11-ba8b-001517c991cf', N'', 1006)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'd5cc02e2-ed9e-df11-a601-001517dd2d44', N'', 1007)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'7aa9e945-192e-e511-bb65-001917bd1d46', N'', 5100)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'286a5a3b-b496-46b5-b8d7-033cce9d32e9', N'', 5036)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'b6ab99e8-5da4-4c33-9edd-08298a1a2104', N'', 0)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'd73b84d4-5e2a-4ea2-9f9d-0916e941b206', N'', 5066)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'28592b49-5215-417a-a6a2-10b4cfeef7ce', N'', 5032)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'28bc43d4-c88f-4f02-a5c7-1285f0530706', N'', 5044)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'37138aba-a01b-442a-86ac-1b060871c98a', N'', 5039)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'703abbc7-7fd2-4161-8081-2d7223c8169a', N'', 5037)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'9420798f-433b-4245-af0b-320c8fd39b7a', N'', 5065)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'ab433ca4-eee2-434e-a4b3-3a73550fee02', N'', 5031)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'658f8c01-1bd0-4a52-9870-4482afbd4655', N'', 5061)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'2157a470-b777-424c-b4a5-5743459b90e5', N'', 5048)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'7e3c436e-29b3-4e85-ba69-70830ecabcd1', N'', 5064)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'9ea66238-b66b-4bce-a5cd-7ed40267d31c', N'', 5046)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'e4898572-0076-49f7-830f-88e37abe6556', N'', 5034)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'161a9328-be23-4c29-b9d6-9c506c9f9f4f', N'', 5063)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'd0700273-3e27-40db-962c-a3dc44af37ba', N'', 5043)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'06b473e6-e7a8-45e4-93bd-b3033cde7c88', N'', 5047)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'2b12ce23-8c6b-4732-8c7a-b9187b0b538e', N'', 5062)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'b67d5432-7b2d-4955-ace1-ba21f56f155f', N'', 5030)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'2d199674-b098-484a-a861-bb0b0e8d1862', N'', 5067)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'fddc4b3c-84f0-4b04-abf8-c19bd58c6a86', N'', 5038)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'66d07173-addf-e111-8706-c82a14062982', N'', 59)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'e38fd47e-addf-e111-8706-c82a14062982', N'', 60)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'02f51487-addf-e111-8706-c82a14062982', N'', 61)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'b5bf249b-addf-e111-8706-c82a14062982', N'', 62)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'4ff9bda6-addf-e111-8706-c82a14062982', N'', 63)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'5b5c35b0-addf-e111-8706-c82a14062982', N'', 64)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'427406c7-addf-e111-8706-c82a14062982', N'SX', 65)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'84e9b6d0-addf-e111-8706-c82a14062982', N'', 66)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'338455da-addf-e111-8706-c82a14062982', N'SN', 67)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'26e39fe3-addf-e111-8706-c82a14062982', N'UO', 68)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'1192dbee-addf-e111-8706-c82a14062982', N'', 69)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'13d0b6f9-addf-e111-8706-c82a14062982', N'', 70)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'65f54d06-aedf-e111-8706-c82a14062982', N'', 71)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'e9135913-aedf-e111-8706-c82a14062982', N'', 72)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'f7fbbf1c-aedf-e111-8706-c82a14062982', N'UQ', 73)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'6f568b24-aedf-e111-8706-c82a14062982', N'', 74)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'4f19482d-aedf-e111-8706-c82a14062982', N'', 75)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'35064552-aedf-e111-8706-c82a14062982', N'', 5001)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'2062af5a-aedf-e111-8706-c82a14062982', N'', 5002)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'9a60b261-aedf-e111-8706-c82a14062982', N'', 5003)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'8e4adb6c-aedf-e111-8706-c82a14062982', N'', 5004)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'346e8874-aedf-e111-8706-c82a14062982', N'', 5005)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'356e8874-aedf-e111-8706-c82a14062982', N'', 5006)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'6ba4c782-aedf-e111-8706-c82a14062982', N'', 5007)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'5bf3438a-aedf-e111-8706-c82a14062982', N'', 5008)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'59c10497-aedf-e111-8706-c82a14062982', N'', 5009)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'f20d089e-aedf-e111-8706-c82a14062982', N'', 5010)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'a35dbca5-aedf-e111-8706-c82a14062982', N'', 5011)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'f1758dac-aedf-e111-8706-c82a14062982', N'', 5012)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'4dd4a6b2-aedf-e111-8706-c82a14062982', N'', 5013)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'90632cb9-aedf-e111-8706-c82a14062982', N'', 5014)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'bce785c0-aedf-e111-8706-c82a14062982', N'', 5015)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'921b08c7-aedf-e111-8706-c82a14062982', N'', 5016)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'09ccaccd-aedf-e111-8706-c82a14062982', N'', 5017)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'509516dc-aedf-e111-8706-c82a14062982', N'', 5018)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'204135e4-aedf-e111-8706-c82a14062982', N'', 5019)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'fdadaceb-aedf-e111-8706-c82a14062982', N'', 5020)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'feadaceb-aedf-e111-8706-c82a14062982', N'', 5021)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'4eb687f5-aedf-e111-8706-c82a14062982', N'', 5022)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'4fb687f5-aedf-e111-8706-c82a14062982', N'', 5023)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'e1f99a03-afdf-e111-8706-c82a14062982', N'', 5024)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'e2f99a03-afdf-e111-8706-c82a14062982', N'', 5025)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'2ad55e10-afdf-e111-8706-c82a14062982', N'', 5026)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'0a51a2f5-b2df-e111-8706-c82a14062982', N'', 5027)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'a7020a3c-62f1-478e-a74d-d66ba82f08e0', N'', 5045)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'0f2197b7-0d82-42c6-b772-debaa8624ad5', N'', 5035)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'0ac85655-231c-e211-97c6-e0f8473ba773', N'', 5028)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'd05b3362-231c-e211-97c6-e0f8473ba773', N'', 5029)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'3a170926-5689-4beb-b6f1-e5f93452553f', N'', 5041)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'30f4a098-245b-46c2-bbc4-ea104f546e56', N'', 5033)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'0606549b-7dff-4219-9c1a-ec57e479b3b0', N'', 5040)
		
		INSERT [dbo].[MappingIncidentTypes] ([IncidentTypeGuid], [ForeignType], [IncidentTypeCode]) VALUES (N'a7d5275e-b5dd-40e1-9ead-fae96b526e6b', N'', 5042)
		
	END	
END