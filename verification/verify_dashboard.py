from playwright.sync_api import sync_playwright
import time

def verify_dashboard():
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        page = browser.new_page()
        try:
            # Go to login
            page.goto("http://localhost:5088/Account/Login")

            # Fill login form
            page.fill("input[name='Input.Email']", "admin@metalflow.com")
            page.fill("input[name='Input.Password']", "Admin123!")
            page.click("button[type='submit']")

            # Wait for redirection to home/dashboard
            page.wait_for_url("http://localhost:5088/", timeout=10000)

            # Wait for dashboard content
            page.wait_for_selector("text=Dashboard")

            # Wait for branch info
            # It might take a moment to load async
            time.sleep(2)
            page.wait_for_selector("text=Headquarters (HQ)", timeout=10000)

            # Take a screenshot
            page.screenshot(path="verification/dashboard.png")
            print("Screenshot taken successfully.")

        except Exception as e:
            print(f"Error: {e}")
            page.screenshot(path="verification/error.png")
        finally:
            browser.close()

if __name__ == "__main__":
    verify_dashboard()
