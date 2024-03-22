# -*- coding: utf-8 -*-
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import Select
from selenium.common.exceptions import NoSuchElementException
from selenium.common.exceptions import NoAlertPresentException
import unittest, time, re

class TestSendLetter(unittest.TestCase):
    def setUp(self):
        #self.wd = webdriver.Firefox()
        self.wd = webdriver.Chrome()
        self.wd.implicitly_wait(30)
        # self.base_url = "https://www.google.com/"
        # self.verificationErrors = []
        # self.accept_next_alert = True
    
    def test_send_letter(self):
        wd = self.wd
        wd.get("https://mail.ru/")
        self.wd.implicitly_wait(5)
        # ERROR: Caught exception [ERROR: Unsupported command [loadVars | sample_data.csv | ]]

        wd.find_element_by_name("login").click()
        wd.find_element_by_name("login").clear()
        wd.find_element_by_name("login").send_keys("pazoviy")
        self.wd.implicitly_wait(5)
        #wd.find_element_by_name("domain").click()
        wd.find_element_by_name("domain").click()
        self.wd.implicitly_wait(10)
        wd.find_element_by_xpath("//button[@type='button']").click()
        self.wd.implicitly_wait(5)
        wd.find_element_by_name("password").click()
        wd.find_element_by_name("password").clear()
        wd.find_element_by_name("password").send_keys("dalfin1972$")
        self.wd.implicitly_wait(5)
        wd.find_element_by_xpath("(//button[@type='button'])[2]").click()
        wd.find_element_by_xpath("//div/div/div/div/div/a/span/span").click()
        # ERROR: Caught exception [unknown command [editContent]]
        wd.find_element_by_xpath("(//input[@value=''])[2]").click()
        wd.find_element_by_xpath("//div[3]/div[3]/div").click()
        self.wd.implicitly_wait(5)
        wd.find_element_by_name("Subject").click()
        wd.find_element_by_name("Subject").clear()
        wd.find_element_by_name("Subject").send_keys("Theme3")
        wd.find_element_by_xpath("//div[5]/div/div/div[2]/div/div").click()
        wd.find_element_by_xpath("//div[5]/div/div/div[2]/div/div[3]").click()
        # ERROR: Caught exception [unknown command [editContent]]
        wd.find_element_by_xpath("//div[2]/div/span/span/span").click()
        wd.find_element_by_xpath("//div[9]/div/div/div[2]").click()
        wd.find_element_by_xpath("//span[3]").click()
        wd.find_element_by_xpath("//a[3]/div[2]").click()
    
    def is_element_present(self, how, what):
        try: self.wd.find_element(by=how, value=what)
        except NoSuchElementException as e: return False
        return True
    
    def is_alert_present(self):
        try: self.wd.switch_to_alert()
        except NoAlertPresentException as e: return False
        return True


    def tearDown(self):
        self.wd.quit()

if __name__ == "__main__":
    unittest.main()
