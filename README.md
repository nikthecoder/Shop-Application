# Shop Application

This project is a shop application developed in C# using Windows Presentation Foundation (WPF). It allows users to view a collection of products, add them to a shopping cart, apply discounts, and perform transactions.

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Tests](#tests)
- [Contributing](#contributing)
- [License](#license)

## Introduction

The application is developed using C# and WPF and is designed to simulate a shop experience. Users can browse a collection of products, add them to a cart, apply promo codes for discounts, and complete transactions.

## Features

- **Product Display:** View available products with details such as name, description, and price.
- **Shopping Cart:** Add products to a shopping cart and manage quantities.
- **Discounts:** Apply promo codes to get discounts on the total purchase.
- **Transaction:** Complete transactions and view receipts.

## Installation

1. Clone the repository.
2. Open the solution file in Visual Studio.
3. Build the project to restore dependencies.

## Usage

1. Run the application.
2. Browse available products.
3. Click "Add to cart" to add products to the cart.
4. Enter a valid promo code for discounts (if available).
5. Click "Pay" to complete the transaction.
6. View the receipt and completed transaction details.

## Tests

The project includes unit tests using MSTest framework for various functionalities:

- `FirstProductNameTest`: Verifies the first product's name loaded from a test CSV file.
- `AllProductDescriptionsTest`: Checks all product descriptions loaded from a test CSV file.
- `CreateCartFileTest`: Tests the creation of a cart file.
- `AddAndWriteToCartFileTest`: Validates the addition of products to the cart and writing to a cart file.
- `SaveAndLoadCartTest`: Tests the saving and loading of the cart.

To run the tests:

1. Open the Test Explorer in Visual Studio.
2. Build the solution.
3. Run all tests.

## Contributing

Contributions are welcome! To contribute:

1. Fork the repository.
2. Create your feature branch: `git checkout -b feature/your-feature-name`.
3. Commit your changes: `git commit -am 'Add some feature'`.
4. Push to the branch: `git push origin feature/your-feature-name`.
5. Create a new Pull Request.

## License

This project is licensed under the [MIT License](https://choosealicense.com/licenses/mit/).
