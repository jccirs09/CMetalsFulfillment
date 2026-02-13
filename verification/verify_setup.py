from playwright.sync_api import sync_playwright, expect

def verify_frontend():
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        page = browser.new_page()

        # 1. Navigate to Home
        print("Navigating to home...")
        page.goto("http://localhost:5000")

        # 2. Check for Title
        expect(page).to_have_title("Home")

        # 3. Check for Setup Gate Banner (should be visible)
        print("Checking for Setup Gate Banner...")
        banner = page.get_by_text("Branch Setup Incomplete")
        expect(banner).to_be_visible()

        # 4. Check for chips in banner
        expect(page.get_by_text("Machines", exact=True)).to_be_visible()

        # 5. Click "Fix Now" link
        print("Clicking Fix Now...")
        page.get_by_role("link", name="Fix Now").click()

        # 6. Verify we are on Setup Dashboard
        print("Verifying Setup Dashboard...")
        expect(page.get_by_role("heading", name="Branch Setup Dashboard")).to_be_visible()

        # Check for card content
        # There should be at least one "Missing"
        expect(page.get_by_text("Missing").first).to_be_visible()
        expect(page.get_by_text("Machines").first).to_be_visible()

        # 7. Take Screenshot
        print("Taking screenshot...")
        page.screenshot(path="verification/verification_setup.png")

        browser.close()

if __name__ == "__main__":
    verify_frontend()
