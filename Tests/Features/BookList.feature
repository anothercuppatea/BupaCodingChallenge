Feature: BookList

Fetch and process a list of book owners and their books to categorise into age categories and book types.
@tag1
Scenario: Retrieve books and return child and adult categories based on age
	Given the book service has the following books
	#need to change this to an object instead.
    """
    [
      {
        "name": "Micheal Scott",
        "age": 40,
        "books": [
          {
            "name": "Somehow I Manage",
            "type": "Paperback"
          },
          {
            "name": "Beet Farming Guide",
            "type": "Hardcover"
          }
        ]
      },
      {
        "name": "Cecilia",
        "age": 14,
        "books": [
          {
            "name": "Alphabets",
            "type": "Hardcover"
          }
        ]
      }
    ]
    """

	When I request the categorised books list
	Then I should recieve the following
	| Category | Books                                      |
	| child    | ["Alphabets"]                              |
	| adult    | ["Beet Farming Guide", "Somehow I Manage"] |

Scenario: Retrieve hardcover books and return child and adult categories based on age
	Given the book service has the following books
    """
    [
      {
        "name": "Micheal Scott",
        "age": 40,
        "books": [
          {
            "name": "Somehow I Manage",
            "type": "Paperback"
          },
          {
            "name": "Beet Farming Guide",
            "type": "Hardcover"
          }
        ]
      },
      {
        "name": "Cecilia",
        "age": 14,
        "books": [
          {
            "name": "Alphabets",
            "type": "Hardcover"
          }
        ]
      }
    ]
    """
	When I request the categorised books list
	Then I should recieve the following
	| Category | Books                  |
	| child    | ["Alphabets"]          |
	| adult    | ["Beet Farming Guide"] | 
