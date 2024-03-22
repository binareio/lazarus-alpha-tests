import time

from selenium import webdriver
driver = webdriver.Chrome(r"C:\Users\hina.murdhani\PycharmProjects\pythonProject3\Browser\chromedriver.exe")
driver.maximize_window()
driver.get("https://www.gmail.com")
driver.find_element_by_id("identifierId").send_keys("murdhaniheena.257@gmail.com")
time.sleep(3)
driver.find_element_by_xpath("//div[@class='VfPpkd-RLmnJb']").click()
time.sleep(3)
driver.find_element_by_name("password").send_keys("Jub@Li8197")
time.sleep(2)
driver.find_element_by_xpath("//span[contains(text(),'Next')][1]").click()
time.sleep(3)
driver.close()