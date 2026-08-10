# Interview Requirements

## Live Coding

Implement the following change requests:

1. I want my employees to have salary data
   1. Make the required changes for Employees to have a Salary column on the Employees page
   2. Update the data ingestion process to load employee data that includes Salary. The salary must be greater than 0

2. The search option on the Jobs page is not working. Can you fix it?
   1. The search feature should allow the user to type in some text and, when clicking the search button, get filtered results on the Jobs page
   2. The searched text should apply to all columns, case insensitive, returning rows if any of the columns data starts with the input text

3. Re-uploading the same employee should update, not duplicate.
   1. Match employees by EmployeeCode
   2. Insert new, update existing; report inserted vs updated counts on the import

4. Employees must reference a real job when data is ingested.
   1. When ingesting employees, I want to make sure that each has a real job from the jobs page
   2. I want employees to be valid if the referenced job is in the uploaded jobs file (in the same ingest job)


## Solution Design 

Talk about the solutions for the following features:

1. When ingestion is complete for a file that I upload, I want to refresh the `Data Upload` table and display a notification on screen. How would you implement this?

2. How would you change the application to handle imports for very large csv files (nore than 1 000 000 rows)?
   1. What are the challenges you will face?

3. Product asks to "remove" employees but keep audit history. How would you design it?

4. How would you know if the event bus is stuck, consumers are down, or imports are failing silently?


## System Design

1. Today messages live in SQLite and are polled every N seconds. How would you redesign for production?

2. Multiple companies share one deployment. How do you isolate uploads, storage, employees, and events?

3. API and consumer must run as multiple instances. What breaks in the current design?

4. Instead of (or in addition to) CSV upload, employees come from a vendor API. How would you design it end to end?
