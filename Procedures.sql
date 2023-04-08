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
declare @sqlTemplate nvarchar(2000) = 'select e.Name, e.LName, e.BirthDate,
												count(*) over() TotalRowsCount,
												m.Name as ManagerName, m.LName as ManagerLName
										from Employee e JOIN Employee m on e.ReportsTo = m.EmployeeId
										where e.Name like coalesce(@Name, '''') + ''%''
											and e.LName like coalesce(@LName, '''') + ''%''
											and e.BirthDate >= coalesce(@BirthDate, ''1900-01-01'')
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
	  set @sqlOrderBy = 'e.Employeeid'
	else if @SortColumn = 'Name'
	  set @sqlOrderBy = 'e.Name'
	else if @SortColumn = 'LName'
	  set @sqlOrderBy = 'e.LName'
	else 
	  set @sqlOrderBy = 'e.Employeeid'
	
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

select e.Name, e.Lname, e.BirthDate,
		count(*) over() TotalRowsCount,
		m.Name as ManagerFirstName, m.LName as ManagerLastName
from Employee e JOIN Employee m on e.ReportsTo = m.EmployeeId