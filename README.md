This project uses my C# and SQL querying skills to put together an app that converts an input to a real-time currency output.

The user can also input their own currency and save it in a locally stored database.

Preview of the application:

<img width="1004" height="594" alt="Screenshot 2026-10-11 215531" src="https://github.com/user-attachments/assets/7c37520a-0adf-4af5-a7f5-781f5f5e9fde" />
<img width="1105" height="627" alt="Screenshot 2026-10-11 215548" src="https://github.com/user-attachments/assets/42053e5b-9743-440e-b93c-ec4ce4aa0388" />

## Running it yourself

The live rates come from [Open Exchange Rates](https://openexchangerates.org/signup/free), which needs a free App ID:

1. In the `CurrencyConverter_Static` folder, copy `secrets.example.config` to `secrets.config`.
2. Replace `YOUR_APP_ID_HERE` with your App ID.

`secrets.config` is in `.gitignore`, so your App ID stays off GitHub.
