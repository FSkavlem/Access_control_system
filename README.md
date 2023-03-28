# Access control system with Posgre SQL DB

The access control system contains 2 programs with GUI.  
The main program Sentral holds 4 threads, one overarching thread the Server that hold the TCP socket and is parent thread for all other threads. All communication between the threads goes through, this thread via delegate handlers and events. The 3 child threads are SQL queries, TCP handler and GUI thread. 

The SQL thread does async calls to the postgres SQL database at my school, the async processes are done via executing tasks. The second thread TCP handler, handles the tcp connections to the various cardreaders in the system. The last thread the GUI thread updates the GUI.
The Cardreader holds 4 threads, one parent thread that holds the 3 child threads cardreader GUI, Cardreader TCPclient and Cardreader Serialclient Threads.

![alt text](https://github.com/FSkavlem/Access_control_system/blob/master/ThePlan-1.png?raw=true)
