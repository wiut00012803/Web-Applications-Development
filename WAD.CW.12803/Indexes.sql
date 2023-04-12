select EmployeeId, Name, LName, BirthDate
from Employee

update employee set birthdate = null where employeeid = 8


declare @Name nvarchar(20) = 'aaa'
declare @LName nvarchar(20) = 'bbb'
declare @BirthDate datetime = '2010-01-01'
insert into Employee(Name, LName, BirthDate)
values(@Name, @LName, @BirthDate)
select @@IDENTITY
select IDENT_CURRENT('Employee')
select SCOPE_IDENTITY();

declare @Name nvarchar(20) = 'updated'
declare @LName nvarchar(20) = 'bbb'
declare @BirthDate datetime = '2010-01-01'
declare @EmployeeId int = 1
update Employee set
  Name = @Name, 
  LName  = @LName, 
  BirthDate  = @BirthDate
where EmployeeId = @EmployeeId
----------------- Stored Procedures for CRUD ---------------------------

go
create proc DatabaseGetAllEmployees
as
begin
  select EmployeeId, Name, LName, BirthDate from Employee
end
go
exec DatabaseGetAllEmployees
go
----Insert --
create proc DatabaseInsertEmployee(
  @Name nvarchar(20), 
  @LName nvarchar(20), 
  @BirthDate datetime,
  @EmployeeId int OUT,
  @Errors varchar(1000) OUT
) as
begin
  begin try
	  insert into Employee(Name, LName, BirthDate)
	  values(@Name, @LName, @BirthDate)
	  select @EmployeeId = SCOPE_IDENTITY()
  end try
  begin catch
    select @Errors = ERROR_MESSAGE()
    return (1)
  end catch
  return (0)
end
go
declare @eid int
declare @err varchar(1000)
declare @retcode int
exec @retcode = DatabaseInsertEmployee @Name = 'aaa', @LName= 'bbb', @BirthDate = '2012-12-16'
		,@EmployeeId = @eid OUT, @Errors = @err OUT
print @eid
print @err
print @retcode
--------------------------
declare @Name nvarchar(20) = null
declare @LName nvarchar(20) = null
declare @BirthDate datetime = null
declare @OffsetRows int  = 0
declare @PageSize int =10
select Employeeid, Name, LName, BirthDate,
	count(*) over() TotalRowsCount
from Employee
where Name like coalesce(@Name, '') + '%'
	and LName like coalesce(@LName, '') + '%'
	and BirthDate >= coalesce(@BirthDate, '1900-01-01')
order by EmployeeId
offset @OffsetRows rows
fetch next @PageSize rows only
---------------------------------------
go
create proc dbsdEmployeeFilter(
 @Name nvarchar(20),
 @LName nvarchar(20),
 @BirthDate datetime ,
 @OffsetRows int  ,
 @PageSize int 
)
as
begin
	select Employeeid, Name, LName, BirthDate,
		count(*) over() TotalRowsCount
	from Employee
	where Name like coalesce(@Name, '') + '%'
		and LName like coalesce(@LName, '') + '%'
		and BirthDate >= coalesce(@BirthDate, '1900-01-01')
	order by EmployeeId
	offset @OffsetRows rows
	fetch next @PageSize rows only
end
------------------
exec dbsdEmployeeFilter @Name= null, @LName = null, @BirthDate = null, @OffsetRows = 2, @PageSize = 2
------------------- Filter with sorting -------------------------
go
create or alter proc dbsdEmployeeFilterWithSorting(
 @FN nvarchar(20),
 @LN nvarchar(20),
 @DOB datetime ,
 @SortColumn varchar(300),
 @SortDesc bit,
 @Page int  ,
 @PageSize int 
)
as
declare @sqlTemplate nvarchar(2000) = 'select Employeeid, Name, LName, BirthDate,
											count(*) over() TotalRowsCount
										from Employee
										where Name like coalesce(@Name, '''') + ''%''
											and LName like coalesce(@LName, '''') + ''%''
											and BirthDate >= coalesce(@BirthDate, ''1900-01-01'')
										order by {0}
										offset @OffsetRows rows
										fetch next @PageSize rows only'
declare @sqlParams nvarchar(1000) = ' @Name nvarchar(20), 
									@LName nvarchar(20), 
									@BirthDate datetime,
									@OffsetRows int,
									@PageSize int'
begin
	declare @sqlOrderBy nvarchar(300) 
	if @SortColumn = 'Employeeid'
	  set @sqlOrderBy = 'Employeeid'
	else if @SortColumn = 'Name'
	  set @sqlOrderBy = 'Name'
	else if @SortColumn = 'LName'
	  set @sqlOrderBy = 'LName'
	else 
	  set @sqlOrderBy = 'Employeeid'
	
	if @SortDesc = 1
		set @sqlOrderBy = @sqlOrderBy + ' DESC '

	declare @sql nvarchar(2000) = replace(@sqlTemplate, '{0}', @sqlOrderBy)
	
	declare @Offset int = (@Page-1)*@PageSize
	exec sp_executesql @sql, @sqlParams,
			@Name = @FN,
			@LName = @LN,
			@BirthDate = @DOB,
			@OffsetRows = @Offset,
			@PageSize = @PageSize
end
--------------
exec dbsdEmployeeFilterWithSorting @FN=null, @LN=null,@DOB=null,
									@SortColumn='Name' , @SortDesc = 1,
									@Page=1, @PageSize=5

---------------- Export ----------

select EmployeeId as "@Id", Name as FN, LName
,Birthdate as "Manager/DOB"
,ReportsTo as "Manager/Id"
from Employee
for xml path('Emp'), root('EmployeesWithManagerInfo')

select * 
from Employee
for json auto

------------------------------
go
create or alter proc dbsdExportEmployeeToXml
as
declare @tab as table (
  Employeeid int, 
  Name nvarchar(20), 
  LName nvarchar(20), 
  BirthDate datetime,
  TotalRowsCount int
)
begin
  insert into @tab
  exec dbsdEmployeeFilterWithSorting @FN=null, @LN=null,@DOB=null,
									@SortColumn='Name' , @SortDesc = 1,
									@Page=1, @PageSize=5

   select * from @tab
   for xml path('EmpRow'), root('Employees')
end
go
exec dbsdExportEmployeeToXml

---------------- Import ------------------
--drop type udtEmpTable
create type udtEmpTable as table (
  Name nvarchar(20),
  LName nvarchar(20),
  BirthDate datetime
)
go
create or alter proc dbsdBulkInsertEmployees(@EmpTable udtEmpTable readonly)
as
begin
  insert into Employee(Name, LName, BirthDate)
  output inserted.* 
  select Name, LName, BirthDate from @EmpTable
end

go
declare @tab udtEmpTable
insert into @tab values('Mike', 'Smith', '1988-01-01'), 
					   ('Kate', 'Smith', '1998-01-05')
exec dbsdBulkInsertEmployees @EmpTable = @tab 
------------------

select Name, LName, BirthDate
from Employee
for json auto
-------------------- Import with parcing at DB side -----------
select Name, LName, BirthDate
from Employee
for xml auto, root('Employees')
go
create or alter proc dbsdImportEmployeesFromXml(@xml nvarchar(MAX))
as
begin
  declare @xmlDoc int

  exec sp_xml_preparedocument @xmlDoc OUT, @xml
 
  insert into Employee( Name, LName, BirthDate)
  output inserted.*
  select Name, LName, BirthDate
  from openxml(@xmlDoc, '/Employees/Employee', 1)
  with (
     Name nvarchar(20),
	 LName nvarchar(20),
	 BirthDate datetime
  )
end
go
exec dbsdImportEmployeesFromXml @xml = '<Employees>
  <Employee Name="Andrew" LName="Adams" BirthDate="1962-02-18T00:00:00" />
  <Employee Name="Nancy" LName="Edwards" BirthDate="1958-12-08T00:00:00" />
  <Employee Name="Jane" LName="Peacock" BirthDate="1973-08-29T00:00:00" />
  <Employee Name="Margaret" LName="Park" BirthDate="1947-09-19T00:00:00" />
  <Employee Name="Steve" LName="Johnson" BirthDate="1965-03-03T00:00:00" />
  <Employee Name="Michael" LName="Mitchell" BirthDate="1973-07-01T00:00:00" />
  <Employee Name="Robert" LName="King" BirthDate="1970-05-29T00:00:00" />
  <Employee Name="Laura" LName="Callahan" BirthDate="1968-01-09T00:00:00" />
  <Employee Name="Mike" LName="Smith" BirthDate="1988-01-01T00:00:00" />
  <Employee Name="Kate" LName="Smith" BirthDate="1998-01-05T00:00:00" />
  <Employee Name="Andrew" LName="Adams" BirthDate="1962-02-18T00:00:00" />
  <Employee Name="Nancy" LName="Edwards" BirthDate="1958-12-08T00:00:00" />
  <Employee Name="Jane" LName="Peacock" BirthDate="1973-08-29T00:00:00" />
  <Employee Name="Margaret" LName="Park" BirthDate="1947-09-19T00:00:00" />
  <Employee Name="Steve" LName="Johnson" BirthDate="1965-03-03T00:00:00" />
  <Employee Name="Michael" LName="Mitchell" BirthDate="1973-07-01T00:00:00" />
  <Employee Name="Robert" LName="King" BirthDate="1970-05-29T00:00:00" />
  <Employee Name="Laura" LName="Callahan" BirthDate="1968-01-09T00:00:00" />
</Employees>'

--------------

create index ix_employee_Name on employee(Name)
select * from employee where Name <> 'Mike'

declare @cnt int = 0
while @cnt < 5000
begin
  set @cnt = @cnt +1
  insert into employee(Name, LName) values(concat('fn',@cnt), concat('ln',@cnt))
end