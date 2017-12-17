# RabbitMQ-Benchmark
A small benchmark for RabbitMQ

Step 1.
-----
To run this project Erlang and RabbitMQ Server needs to be installed and running.

Step 2.
-----
Add an admin user with all permission - Username: test and password test.

Step 3.
-----
Either compile the project yourself or use the compiled version in the folder "Compiled". 

Step 4.
-----
# To run the Client-Server/Cluster test on your local machine.

> goto "Compiled/RabbitMQ.Receiver" and run "Run_10k.bat"

> goto "Compiled/RabbitMQ.Sender" and run "Run.bat"

# To run the Connection disruption test on your local machine.

> goto "Compiled/RabbitMQ.Receiver" and run "Run_SenderTimed.bat"

> goto "Compiled/RabbitMQ.SenderTimed" and run "Run.bat"
