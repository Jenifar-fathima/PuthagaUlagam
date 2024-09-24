﻿using PuthagaUlagam.Common;
using PuthagaUlagam.Logic;
using System;
using System.Linq;

namespace PuthagaUlagam
{
    public partial class AddUpdateBook : System.Web.UI.Page
    {
        private readonly BookOperationBL operationBL = new BookOperationBL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["ISBN"] != null)
                {
                    int isbn = (int)Session["ISBN"];
                    Book book = operationBL.GetBookByIsbn(isbn);
                    if (book != null)
                    {
                        lblAddOrUpdateTitle.Text = "Update Book";
                        txtTitle.Text = book.Title;
                        txtAuthor.Text = book.Author;
                        txtISBN.Text = book.ISBN.ToString();
                        txtPrice.Text = book.Price.ToString();
                        DateOfPublication.SelectedDate = book.Date;
                        DisplayDate.Text = book.Date.ToString("d");
                        txtBookCount.Text = book.Count.ToString();
                        btnAdd.Visible = false;
                        btnUpdate.Visible = true;
                        txtISBN.ReadOnly = true;
                    }

                    Session["ISBN"] = null;
                }
                else
                {
                    lblAddOrUpdateTitle.Text = "Add Book";
                    btnUpdate.Visible = false;
                    btnAdd.Visible = true;
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            HandleBookOperation(OperationType.Add);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            HandleBookOperation(OperationType.Update);
        }

        private void HandleBookOperation(OperationType operationType)
        {
            lblErrorMessage.Text = " ";

            if (IsInputValid())
            {
                BookDTO bookdto = CreateBookDTO();

                if (operationType == OperationType.Add)
                {
                    int nIsbn = int.Parse(txtISBN.Text);
                    ApiResponse<bool> uniqueISBN = operationBL.UniqueIsbnValidation(nIsbn);
                    
                    if (uniqueISBN.IsSuccess)
                    {
                        ApiResponse<bool> addBook = operationBL.AddBook(bookdto);
                        lblErrorMessage.Text = addBook.Message;
                    }
                    lblErrorMessage.Text = uniqueISBN.Message;
                }

                else if (operationType == OperationType.Update)
                {
                    ApiResponse<bool> updateBook = operationBL.UpdateBook(bookdto);
                    lblErrorMessage.Text = updateBook.Message;

                    Response.Redirect("ViewBook.aspx");
                }
                ClearDataField();
            }
            else
            {
                InputValidationMessageMethod();
            }
        }


        private BookDTO CreateBookDTO()
        {
            return new BookDTO
            {
                Title = txtTitle.Text,
                Author = txtAuthor.Text,
                ISBN = int.Parse(txtISBN.Text),
                Price = decimal.Parse(txtPrice.Text),
                Date = DateOfPublication.SelectedDate,
                Count = int.Parse(txtBookCount.Text)
            };
        }

        private bool IsInputValid()
        {
            return  InputValidation.IsValidISBN(txtISBN.Text) &&
                    InputValidation.IsValidCount(txtBookCount.Text) &&
                    InputValidation.IsValidPrice(txtPrice.Text) &&
                    InputValidation.IsValidDateOfPublication(DateOfPublication.SelectedDate);
        }

        private void ClearDataField()
        {
            txtTitle.Text = string.Empty;
            txtAuthor.Text = string.Empty;
            txtISBN.Text = string.Empty;
            txtPrice.Text = string.Empty;
            DateOfPublication.SelectedDate = DateTime.Now;
            DisplayDate.Text = string.Empty;
            txtBookCount.Text = string.Empty;
        }

        protected void PickTheDateOfPublication_Click(object sender, EventArgs e)
        {
            DateOfPublication.Visible = !DateOfPublication.Visible;
        }

        protected void Calender_date_changed(object sender, EventArgs e)
        {
            DateOfPublication.Visible = false;
            DisplayDate.Text = "Selected Date: " + DateOfPublication.SelectedDate.ToString("d");
        }

        private void InputValidationMessageMethod()
        {
            if (!InputValidation.IsValidISBN(txtISBN.Text))
            {
                lblErrorMessage.Text += Messages.InvalidISBN;
            }
            if (!InputValidation.IsValidCount(txtBookCount.Text))
            {
                lblErrorMessage.Text += Messages.InvalidCount;
            }
            if (!InputValidation.IsValidPrice(txtPrice.Text))
            {
                lblErrorMessage.Text += Messages.InvalidPrice;
            }
            if (!InputValidation.IsValidDateOfPublication(DateOfPublication.SelectedDate))
            {
                lblErrorMessage.Text += Messages.InvalidDate;
            }
        }
    }
}