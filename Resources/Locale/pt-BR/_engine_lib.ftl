# Used internally by the THE() function.
zzzz-the = { PROPER($ent) ->
    *[false] a { $ent }
     [true] { $ent }
    }

# Used internally by the SUBJECT() function.
he-subject = ele
she-subject = ela
they-subject = eles
it-subject = ele

# Used internally by the OBJECT() function.
he-object = ele
she-object = ela
they-object = eles
it-object = isso

# Used internally by the POSS-PRONOUN() function.
he-poss-pronoun = dele
she-poss-pronoun = dela
they-poss-pronoun = deles
it-poss-pronoun = dele

# Used internally by the POSS-ADJ() function.
he-poss-adjective = dele
she-poss-adjective = dela
they-poss-adjective = deles
it-poss-adjective = dele

# Used internally by the REFLEXIVE() function.
he-reflexive = ele mesmo
she-reflexive = ela mesmo
they-reflexive = eles mesmos
it-reflexive = ele mesmo

# Used internally by the CONJUGATE-BE() function.
be-plural = é
be-singular = é

# Used internally by the CONJUGATE-HAVE() function.
have-plural = tenho
have-singular = tem
