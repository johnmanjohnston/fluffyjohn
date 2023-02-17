#include <iostream>
#include <chrono>
#include <stdlib.h>
#include <string>
#include <thread>

int iteration = 0;
int iterationCap = -1; // Run infinitely 
bool countLogs = true;

void Sleep() 
{
    std::this_thread::sleep_for(std::chrono::seconds(5));
}

void RecursiveRun() 
{
    if (iteration >= iterationCap) 
    {
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

int main() 
{
    RecursiveRun();
}
