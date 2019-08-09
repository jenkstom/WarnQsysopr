# WarnQsysopr
Look for unanswered qsysopr messages on an IBMi / iSeries server and send email.

This program is meant to be run on a scheduler once or more per hour. 
Change information about the email server, iSeries, username and password
before using. Please also change the destination and origin email addresses.
Check both the program.cs and db.cs files, both of them need information
changed before this will work for you.

This was created with the old client access package. If you are using the
newer IBM i Access Client Solutions see below:

https://www-01.ibm.com/support/docview.wss?uid=isg3T1026805

You'll want the "Connectivity to DB2® for i using ODBC, .Net, OLE DB and XDA"
package for the newer Client Solutions.

You may need to change the ODBC DSN in the db.cs file for it to work. I have
no experience with it.
