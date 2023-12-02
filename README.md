# Desktop App Like - React + ASP.NET Core WebAPI

## What Am I Going To Talk about Here?
For the past 2 weeks, I have been thinking about creating a tool that can be shared with my friends at work.

My tool is a query runner, which means we can write multiple queries in it and it connects to DB, retrieves the result, and shows it in a grid, pretty simple but useful.

Then I thought about it... What should be the UI format? I want my friends to use it seamlessly like during their busy workday. Simply double-click the exe, Do the stuff, close. That's it

### Hunt For A UI Format...

Now I started thinking. How about a desktop app? Yes, it can run so fast and is quick to launch, do stuff, and close. Best for my scenario. So I have few options
1. WPF
2. WinForms
3. Electron
4. MAUI
5. Other emulation systems...

- First of all, This is for a work environment so my colleagues don't have Node and stuff installed to run. And they cannot do without raising administrative requests. Now this option is off the plate right away.
- Secondly, WPF, To be honest, I don't like that from the start.
- Third MAUI is pretty and shiny new but buggy at the moment. Also, less documentation if I became stuck somewhere I'm done
- Other sorts of emulations need some kind of admin access to install the SDK first

So what's preinstalled with my work system is WinForms. And it's a legacy but much-loved framework ..!
So I started building an app in WinForms and shared it with the team, It worked well all good.

### WinForms Worked Great ..!
### But has a lot of limitations from a UI point of view. It is customizable only to a limit and has an older look and feel.

That's when I decided to go for a desktop's versatility and a Web UI's customisability.

## So I go for React + ASP.NET Core WebAPI embedded in a WinForms UI shell
It gives me the following big advantages
- UI that runs on React... Exciting because I can design performant and totally customizable UI now
- Backed by ASP.NET Core... Again exciting why? Because the UI can send a request to the backend and it can do everything natively without bridging just like a C# console app
- WinForms shell... No more relying on web browsers that show context menus and stuff. Now it can just look like my desktop app but hosts a React+ASP.NET Core WebAPI behind the screens.

# It looks something like this

![image](https://github.com/sangeethnandakumar/DeskKiosk/assets/24974154/b0aa5556-2504-40f3-89e7-9ec28bfaa2ff)

# Get Started
### To start with Create a new project with React + ASP.NET Core Template. You can use Js or TypeScript (I prefer Js)

> MAKE SURE TO NOT USE HTTPS. UNCHECK IT OTHERWISE IT'S A MESS LATER FOR MANY THINGS. USE IT IF YOU ARE SURE YOUR CLIENTS HAVE PRIVILEGES TO TRUST CERTIFICATES. IT'S OFTEN A MESS SO I'M NEVER GOING TO USE HTTPS


![image](https://github.com/sangeethnandakumar/DeskKiosk/assets/24974154/6da38a9e-c126-40fe-88f2-bea56139eb11)

### This will create 2 projects. I named my project "DeskKiosk". So VS created 2 projects
1. DeskKiosk.Server (With ASP.NET)
2. DeskKiosk.Client (React SPA)

### Then I added a "Windows Forms App" project to the same solution. I named it "App". So now I have 3 projects in my solution
1. DeskKiosk.Server (With ASP.NET)
2. DeskKiosk.Client (React SPA)
3. App (WinForms)
   
![image](https://github.com/sangeethnandakumar/DeskKiosk/assets/24974154/f782a489-7673-4fcb-9005-026b67c6a4c9)

# Step 1: Rewrite ViteConfig
Since we disabled HTTPS for the project, Let's rewrite "vite.config.js" file on SPA project (Vite's config) as follows:

```js
import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [plugin()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/weatherforecast': {
                target: 'http://localhost:5173',
                secure: false
            }
        },
        cors: false,
        port: 5173,
        https: false,
    }
});
```

> Here, We're telling for development purposes to launch Vite server on `http://localhost:5173` and disable HTTPS. DONE with frontend..!

# Step 2: Rewrite Program.cs
Now comes the WebAPI part. here configure Program.cs as follows:

### POINT THE WEBROOT TO THE DIST FOLDER OF YOUR SPA. This is mandatory for resolving static files if debugging

```csharp
using DeskKiosk.Server.Infrastructure;
using System.Net;

namespace DeskKiosk.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                WebRootPath = @"C:\Users\Sangeeth Nandakumar\source\repos\DeskKiosk\deskkiosk.client\dist"
            });
            builder.WebHost.UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 5000);
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<SignalRHub>();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(builder => builder
                .WithOrigins("http://localhost:5173", "http://127.0.0.1:5000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );
            app.UseAuthorization();
            app.MapControllers();
            app.MapFallbackToFile("/index.html");
            app.MapHub<SignalRHub>("/signalrhub");

            app.Run();
        }
    }
}
```

> Notice we're not setting any CORS policies. This is to have flexibility since we're creating a desktop app. I'm not bothered about security since it's a local utility app only which I'm going to share with my friends.
> I'm keeping Swagger for whatever environment
> See the Kestral part, Kestral has an implicit redirect to HTTPS. I don't want it. So disabled. DONE..! We're now good with the backend part.

 # Step 3: Test & Publish
We're good. Now implement your logic in the frontend and backend. Test and ensure you're happy.
All cool? Ok then right-click on WebAPI project and Publish. DONE ..!

 # Step 4: Setup WinForms Project
We already added the WinForms project, Now simply do the following:

1. Install CefSharp NuGet package - `CefSharp.WinForms`. This will give us the latest updated Chromium-based web browser control
2. Open your Form UI
3. Goto tool window and drag and drop the browser control

![image](https://github.com/sangeethnandakumar/DeskKiosk/assets/24974154/5f45c3ea-aab8-44f5-97e3-71bf616206bd)

4. Now click on the control, and its property window appears. Set the Dock to "Fill"
5. Name your control "Browser"
6. Let's name your Form "MyApp" or something you prefer
7. Now double-click the form and go code-behind
8. Simply add below:

```csharp
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App
{
    public partial class Main : Form
    {
        const string SERVER_URL = "http://127.0.0.1:5000/";
        const string SERVER_LOC = @"C:\Users\Sangeeth Nandakumar\source\repos\DeskKiosk\DeskKiosk.Server\bin\Release\net8.0\publish";
        const string EXE_NAME = "DeskKiosk.Server.exe";

        public Main()
        {
            InitializeComponent();
            Browser.MenuHandler = new InvisibleMenuHandler();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if (IsServerRunning())
                {
                    Browser.Load(SERVER_URL);
                }
                else
                {
                    StartServer();
                    Browser.Load(SERVER_URL);
                }
            });
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (IsLastInstance())
            {
                TerminateServer();
            }
        }

        private void StartServer()
        {
            if (!IsServerRunning())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(SERVER_LOC, EXE_NAME),
                    WorkingDirectory = SERVER_LOC,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };

                Process process = new Process
                {
                    StartInfo = startInfo
                };

                process.Start();
            }
        }

        private void TerminateServer()
        {
            string processName = Path.GetFileNameWithoutExtension(EXE_NAME);
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                    Console.WriteLine($"Process {process.ProcessName} terminated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error terminating process {process.ProcessName}: {ex.Message}");
                }
            }
        }

        private bool IsServerRunning()
        {
            string processName = Path.GetFileNameWithoutExtension(EXE_NAME);
            Process[] processes = Process.GetProcessesByName(processName);

            return processes.Length > 0;
        }

        private bool IsLastInstance()
        {
            // Check if this is the last instance of the application running
            return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length == 1;
        }
    }
}
```

> WE HAVE 2 EVENTS ABOVE. FormLoad and FormClosed.

> Every time our app loads, it restarts the server. Every time you close your app it terminates the server so it won't run in the background without noticing

## Now when you launch your WinForms app, It tries to launch the ASP.NET Core server in the background, And the server outputs the static react published artifacts directly to the browser. It works without the user noticing anything. Cherry on top.. It's fast..! Fast as ASP.NET Core ..!

So this could be your workflow
1. Build and test everything frontend and backend within Visual Studio
2. Once you're all set Publish WebAPI
3. Copy WebAPI published folder's inner files
4. Then goto WinForms projects Debug folder
5. Paste there
6. Now WinForms project has Server exe within it's working directory
7. Now run WinForms project
8. It will work ..!

## Few Other Fixes
Everything works... But when you right-click on the webview you may see this context menu.Not sure if you like it but I don't. This give the users ability to print or lookup the HTML source code. Not something I prefer considering this as a desktop app.

![image](https://github.com/sangeethnandakumar/DeskKiosk/assets/24974154/c168fe42-7ff4-492e-8dbc-cc88aa2081ec)

So I need to get rid of this context menu.

So I created an `InvisibleMenuHandler.cs` class

```csharp
using CefSharp;

namespace App
{
    public class InvisibleMenuHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
}
```

Then used it like this:

```csharp
 public partial class Main : Form
 {
     public Main()
     {
         InitializeComponent();
         Browser.MenuHandler = new InvisibleMenuHandler();
     }

      //Rest of your code...
```

# Communication

For communication between UI and the backend, We will rely on SignalR. A much much faster reliable way of real-time communication. This is essential because if you prefer to use API controllers although possible may seem a little slow as the HTTP connections need to resolve. By using SignalR, we leverage on multiple transports like WebSockets to polling to have a realtime communication.

To start with, Change the files as follows:

## SignalR Hub

```csharp
using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace DeskKiosk.Server.Infrastructure
{
    public class SignalRHub : Hub
    {
        public async Task InvokeAsync(string component, string methodName, object payload)
        {
            await Clients.All.SendAsync($"{component}_{methodName}", payload);
        }

        public async Task AppPage_OnTextChange(string message)
        {
            await InvokeAsync("AppPage", "OnTextChange", Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
            if(message.Length > 5)
            {
                await InvokeAsync("AppPage", "OnAlert", Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
            }
        }
    }
}
```

## Latch.js

To avoid polluting the UI, The best idea is to move SignalR mechanics to a file called `latch.js`. It keeps the connection and exposes  methods `on` and `invoke`. Use accordingly

```js
// latch.js controls incomming and outgoing communications using SignalR as backbone

import { HubConnectionBuilder } from "@microsoft/signalr";

class Latch {
    constructor() {
        this.connection = null;
        this.initializeConnection();
    }

    initializeConnection() {
        this.connection = new HubConnectionBuilder()
            .withUrl("http://127.0.0.1:5000/signalrhub")
            .withAutomaticReconnect()
            .build();

        this.connection.start().catch((error) => console.log(error));
    }

    on(component, methodName, callback) {
        this.connection.on(`${component}_${methodName}`, callback);
    }

    invoke(component, methodName, ...args) {
        this.connection.invoke(`${component}_${methodName}`, ...args);
    }
}

const latch = new Latch();

export default latch;
```

## App.jsx

Once `Latch.js` is ready, We can simply use to to publish and subscribe to events. You can sent any objects you want to and from server easily blazingly fast and aid communication.

```jsx
// App.js

import { useState, useEffect } from 'react';
import latch from './latch';

function App() {
    const [text, setText] = useState('');
    const [encodedText, setEncodedText] = useState('');

    useEffect(() => {
        latch.on("AppPage", "OnTextChange", msg => setEncodedText(msg));
        latch.on("AppPage", "OnAlert", msg => alert(msg));
    }, []);

    const onTextChange = (e) => {
        setText(e.target.value);
        latch.invoke("AppPage", "OnTextChange", e.target.value);
    }

    return (
        <div>
            <h1>Base64 Encoder / Decoder</h1>
            <textarea rows="5" cols="100" placeholder="Normal Text" value={text} onChange={onTextChange}></textarea>
            <br />
            <br />
            <textarea rows="5" cols="100" disabled placeholder="Encoded Text" value={encodedText}></textarea>
        </div>
    );
}

export default App;
```
