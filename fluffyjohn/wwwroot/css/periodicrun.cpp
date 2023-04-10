#include <iostream>
#include <chrono>
#include <stdlib.h>
#include <string>
#include <thread>
#include <signal.h>

int iteration;
int iterationCap = 200;
int sleepTime = 1;

bool capIterations = false;
bool countLogs = true;

// Exit handler
void HandleExit(int sig) {
    std::cout << "Exiting with " << iteration << " log(s)";
    exit(0);
}

void Sleep() 
{
    std::this_thread::sleep_for(std::chrono::seconds(sleepTime));
}

void RecursiveRun() 
{
    // Verify if we want to run
    if (iteration >= iterationCap && capIterations) 
    {
        std::cout << "Exiting with " << iteration << " log(s)";
        exit(0);
    }

    iteration++;

    if (countLogs) 
    {
        std::cout << "Running iteration " << iteration << "...\n";
    }

    system("sass storage.scss storage.css");
    Sleep();

    RecursiveRun();
}

int main(int argc, char *argv[])
{
    signal(SIGINT, HandleExit);

    // 0              1           2
    // ./periodic-run <sleepTime> <iterationCap>

    if (argc < 2) 
    {
        std::cout << "Insufficient arguments, using default values\n";
    } 

    else 
    {
        sleepTime = std::stoi(argv[1]);
        iterationCap = std::stoi(argv[2]);
    }

    std::cout << "sleepTime: " << sleepTime << "; iterationCap: " << iterationCap << "\n";

    RecursiveRun();
}
